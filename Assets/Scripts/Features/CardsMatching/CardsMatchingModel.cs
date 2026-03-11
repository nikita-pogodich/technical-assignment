using System;
using System.Collections.Generic;
using System.Threading;
using Core.ModelProvider;
using Core.MVPImplementation;
using Core.SaveSystem;
using Cysharp.Threading.Tasks;
using R3;
using Settings;

namespace Features.CardsMatching
{
    public class CardsMatchingModel : BaseModel
    {
        private readonly ILocalSettings _localSettings;
        private readonly IModelProvider _modelProvider;
        private readonly ISaveSystem _saveSystem;

        private readonly ReactiveProperty<GameState> _currentGameState = new();
        private readonly ReactiveProperty<int> _currentScore = new();
        private readonly ReactiveProperty<int> _comboMultiplier = new();
        private readonly ReactiveCommand _cardsMatched = new();
        private readonly ReactiveCommand _cardsMismatched = new();
        private readonly ReactiveCommand _comboMultiplierIncreased = new();
        private readonly ReactiveCommand _comboMultiplierLost = new();
        private readonly Dictionary<int, CardModel> _currentCardModelByPositions = new();
        private readonly List<int> _randomCardTypeIndices = new();
        private readonly Stack<int> _availableCardTypeIndices = new();
        private readonly List<int> _randomCardIndices = new();
        private readonly Stack<int> _availableCardIndices = new();
        private readonly List<CardModel> _flippedCards = new();
        private readonly List<CardModel> _matchedCards = new();
        private readonly List<CardModel> _resettingMismatchedCards = new();
        private readonly Random _random = new();

        private int _cardsMatchedAmount = 0;
        private CompositeDisposable _cardsSelectionDisposable = new();
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationTokenSource _mismatchedCardsResettingCancellationTokenSource;

        public IReadOnlyDictionary<int, CardModel> CurrentCardModelByPositions => _currentCardModelByPositions;
        public ReadOnlyReactiveProperty<GameState> CurrentGameState => _currentGameState;
        public ReadOnlyReactiveProperty<int> CurrentScore => _currentScore;
        public ReadOnlyReactiveProperty<int> ComboMultiplier => _comboMultiplier;
        public Observable<Unit> CardsMatched => _cardsMatched;
        public Observable<Unit> CardsMismatched => _cardsMismatched;
        public Observable<Unit> ComboMultiplierIncreased => _comboMultiplierIncreased;
        public Observable<Unit> ComboMultiplierLost => _comboMultiplierLost;
        public int CurrentStageIndex { get; private set; }

        public CardsMatchingModel(
            ILocalSettings localSettings,
            IModelProvider modelProvider,
            ISaveSystem saveSystem,
            int uniqueId) : base(uniqueId)
        {
            _localSettings = localSettings;
            _modelProvider = modelProvider;
            _saveSystem = saveSystem;
        }

        protected override UniTask OnInit()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            for (int i = 0; i < _localSettings.GameSettings.CardResourceKeys.Count; i++)
            {
                _randomCardTypeIndices.Add(i);
            }

            return UniTask.CompletedTask;
        }

        protected override void OnDeinit()
        {
            DisposeCancellationTokens();
        }

        public void StartNewGame()
        {
            _comboMultiplier.Value = 0;
            _currentScore.Value = 0;
            CurrentStageIndex = -1;
            StartNextStage();
        }

        public void ContinueGame()
        {
            _saveSystem.TryLoad(_localSettings.GameSettings.AutoSaveSlotName, out CardMatchingSaveData saveData);
            _currentScore.Value = saveData.Score;
            _comboMultiplier.Value = saveData.ComboMultiplier;
            CurrentStageIndex = saveData.StageIndex;
            StartNextStage();
        }

        public void StartNextStage()
        {
            if (CurrentStageIndex + 1 >= _localSettings.GameSettings.StageSettings.Count)
            {
                return;
            }

            SaveGame();

            CurrentStageIndex++;

            StageSetting stageSetting = _localSettings.GameSettings.StageSettings[CurrentStageIndex];

            StartStage(stageSetting);
        }

        public void EndGame()
        {
            DisposeCancellationTokens();
            _cardsSelectionDisposable?.Dispose();
            _currentGameState.Value = GameState.GameEnded;
        }

        public async UniTask CompleteCardsCreationAsync()
        {
            _currentGameState.Value = GameState.Remembering;
            var gameSettingsStageSetting = _localSettings.GameSettings.StageSettings[CurrentStageIndex];

            await UniTask.WaitForSeconds(
                gameSettingsStageSetting.TimeToRememberCardsSeconds,
                cancellationToken: _cancellationTokenSource.Token);

            _currentGameState.Value = GameState.CardsHiding;
        }

        public void CompleteCardsHiding()
        {
            _currentGameState.Value = GameState.Matching;
        }

        private void SaveGame()
        {
            var cardMatchingSaveData = new CardMatchingSaveData(
                CurrentStageIndex,
                _currentScore.Value,
                _comboMultiplier.Value);

            _saveSystem.Save(_localSettings.GameSettings.AutoSaveSlotName, cardMatchingSaveData);
        }

        private void StartStage(StageSetting stageSetting)
        {
            _cardsMatchedAmount = 0;

            DisposeCancellationTokens();

            _cancellationTokenSource = new CancellationTokenSource();
            _cardsSelectionDisposable?.Dispose();
            _cardsSelectionDisposable = new CompositeDisposable();
            _currentCardModelByPositions.Clear();
            _availableCardTypeIndices.Clear();
            _availableCardIndices.Clear();
            _randomCardIndices.Clear();
            _matchedCards.Clear();
            _flippedCards.Clear();

            for (int i = 0; i < stageSetting.CardsAmount; i++)
            {
                _randomCardIndices.Add(i);
            }

            ShuffleList(_randomCardTypeIndices);
            ShuffleList(_randomCardIndices);

            foreach (int index in _randomCardTypeIndices)
            {
                _availableCardTypeIndices.Push(index);
            }

            foreach (int index in _randomCardIndices)
            {
                _availableCardIndices.Push(index);
            }

            //TODO: Add check if AmountOfCards can be divided by CardsToMatch
            int amountOfCardTypes = stageSetting.CardsAmount / stageSetting.CardsToMatch;

            for (int i = 0; i < amountOfCardTypes; i++)
            {
                int index = _availableCardTypeIndices.Pop();

                if (index >= _localSettings.GameSettings.CardResourceKeys.Count)
                {
                    //TODO: Add Log
                    continue;
                }

                string resourceKey = _localSettings.GameSettings.CardResourceKeys[index];

                for (int j = 0; j < stageSetting.CardsToMatch; j++)
                {
                    int position = _availableCardIndices.Pop();
                    var cardModel = new CardModel(position, index, resourceKey, _modelProvider.GetUniqueId());
                    cardModel.Selected.Subscribe(OnCardSelected).AddTo(_cardsSelectionDisposable);
                    _currentCardModelByPositions.Add(position, cardModel);
                }
            }

            _currentGameState.Value = GameState.CardsCreation;
        }

        private void OnCardSelected(int position)
        {
            if (_currentGameState.Value != GameState.Matching)
            {
                return;
            }

            if (_currentCardModelByPositions.TryGetValue(position, out CardModel cardModel) == false)
            {
                //TODO: Add Log
                return;
            }

            if (cardModel.IsFlipped.Value || cardModel.IsMatched.Value)
            {
                return;
            }

            if (_resettingMismatchedCards.Count > 0)
            {
                _mismatchedCardsResettingCancellationTokenSource?.Cancel();
                _mismatchedCardsResettingCancellationTokenSource?.Dispose();
                _mismatchedCardsResettingCancellationTokenSource = null;

                ResetMismatchedCards();
            }

            cardModel.IsFlipped.Value = true;
            _flippedCards.Add(cardModel);

            if (_flippedCards.Count > 1)
            {
                CardModel previousFlippedCardModel = _matchedCards[^1];
                if (previousFlippedCardModel.Index == cardModel.Index)
                {
                    _matchedCards.Add(cardModel);
                }
                else
                {
                    _resettingMismatchedCards.Clear();
                    _resettingMismatchedCards.AddRange(_flippedCards);

                    _matchedCards.Clear();
                    _flippedCards.Clear();

                    _mismatchedCardsResettingCancellationTokenSource = new CancellationTokenSource();
                    ResetMismatchedCardsAsync().Forget();

                    if (_comboMultiplier.Value > 1)
                    {
                        _comboMultiplierLost.Execute(Unit.Default);
                    }

                    _comboMultiplier.Value = 0;

                    OnCardMismatched();

                    int mismatchScorePenalty = _localSettings.GameSettings.MismatchScorePenalty;
                    if (_currentScore.Value - mismatchScorePenalty < 0)
                    {
                        _currentScore.Value = 0;
                    }
                    else
                    {
                        _currentScore.Value -= mismatchScorePenalty;
                    }

                    return;
                }

                StageSetting stageSetting = _localSettings.GameSettings.StageSettings[CurrentStageIndex];
                if (_matchedCards.Count == stageSetting.CardsToMatch)
                {
                    foreach (CardModel flippedCardModel in _flippedCards)
                    {
                        flippedCardModel.IsMatched.Value = true;
                    }

                    _comboMultiplier.Value++;

                    if (_comboMultiplier.Value > _localSettings.GameSettings.MaxComboMultiplier)
                    {
                        _comboMultiplier.Value = _localSettings.GameSettings.MaxComboMultiplier;
                    }

                    if (_comboMultiplier.Value > 1)
                    {
                        _comboMultiplierIncreased.Execute(Unit.Default);
                    }

                    OnCardMatched();

                    _cardsMatchedAmount += stageSetting.CardsToMatch;
                    _matchedCards.Clear();
                    _flippedCards.Clear();

                    _currentScore.Value += _localSettings.GameSettings.MatchScoreBonus * _comboMultiplier.Value;

                    if (_cardsMatchedAmount == stageSetting.CardsAmount)
                    {
                        if (CurrentStageIndex + 1 == _localSettings.GameSettings.StageSettings.Count)
                        {
                            SetAllStageCompletedAsync().Forget();
                        }
                        else
                        {
                            SetStageCompletedAsync().Forget();
                        }
                    }
                }
            }
            else
            {
                _matchedCards.Add(cardModel);
            }
        }

        private void OnCardMismatched()
        {
            _cardsMismatched.Execute(Unit.Default);
        }

        private void OnCardMatched()
        {
            _cardsMatched.Execute(Unit.Default);
        }

        private async UniTaskVoid SetStageCompletedAsync()
        {
            await UniTask.WaitForSeconds(
                _localSettings.GameSettings.DelayBeforeStageCompletedSeconds,
                cancellationToken: _cancellationTokenSource.Token);

            _currentGameState.Value = GameState.StageCompleted;
        }

        private async UniTaskVoid SetAllStageCompletedAsync()
        {
            await UniTask.WaitForSeconds(
                _localSettings.GameSettings.DelayBeforeStageCompletedSeconds,
                cancellationToken: _cancellationTokenSource.Token);

            _currentGameState.Value = GameState.AllStagesCompleted;
        }

        private async UniTaskVoid ResetMismatchedCardsAsync()
        {
            await UniTask.WaitForSeconds(
                _localSettings.GameSettings.TimeToResetMismatchedCardsSeconds,
                cancellationToken: _mismatchedCardsResettingCancellationTokenSource.Token);

            ResetMismatchedCards();
        }

        private void ResetMismatchedCards()
        {
            foreach (CardModel cardModel in _resettingMismatchedCards)
            {
                cardModel.IsFlipped.Value = false;
            }

            _resettingMismatchedCards.Clear();
        }

        private void DisposeCancellationTokens()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

            _mismatchedCardsResettingCancellationTokenSource?.Cancel();
            _mismatchedCardsResettingCancellationTokenSource?.Dispose();
            _mismatchedCardsResettingCancellationTokenSource = null;
        }

        private void ShuffleList(List<int> list)
        {
            int index = list.Count;
            while (index > 1)
            {
                index--;
                int k = _random.Next(index + 1);
                (list[k], list[index]) = (list[index], list[k]);
            }
        }
    }
}