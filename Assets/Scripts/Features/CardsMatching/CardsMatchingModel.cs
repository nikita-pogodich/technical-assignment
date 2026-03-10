using System;
using System.Collections.Generic;
using Core.ModelProvider;
using Core.MVPImplementation;
using Cysharp.Threading.Tasks;
using R3;
using Settings;

namespace Features.CardsMatching
{
    public class CardsMatchingModel : BaseModel
    {
        private readonly ILocalSettings _localSettings;
        private readonly IModelProvider _modelProvider;

        private readonly ReactiveProperty<GameState> _currentGameState = new();
        private readonly Dictionary<int, CardModel> _currentCardModelByPositions = new();
        private readonly List<int> _randomCardTypeIndices = new();
        private readonly Stack<int> _availableCardTypeIndices = new();
        private readonly List<int> _randomCardIndices = new();
        private readonly Stack<int> _availableCardIndices = new();
        private readonly List<CardModel> _flippedCards = new();
        private readonly Random _random = new();

        private int _cardsMatched = 0;

        public IReadOnlyDictionary<int, CardModel> CurrentCardModelByPositions => _currentCardModelByPositions;
        public ReadOnlyReactiveProperty<GameState> CurrentGameState => _currentGameState;
        public int CurrentStageIndex { get; private set; }

        public CardsMatchingModel(
            ILocalSettings localSettings,
            IModelProvider modelProvider,
            int uniqueId) : base(uniqueId)
        {
            _localSettings = localSettings;
            _modelProvider = modelProvider;
        }

        public void Init()
        {
            for (int i = 0; i < _localSettings.GameSettings.CardResourceKeys.Count; i++)
            {
                _randomCardTypeIndices.Add(i);
            }
        }

        public void StartNewGame()
        {
            CurrentStageIndex = -1;
            StartNextStage();
        }

        public void StartNextStage()
        {
            CurrentStageIndex++;

            StageSetting stageSetting;
            if (CurrentStageIndex >= _localSettings.GameSettings.StageSettings.Count)
            {
                stageSetting = _localSettings.GameSettings.StageSettings[^1];
            }
            else
            {
                stageSetting = _localSettings.GameSettings.StageSettings[CurrentStageIndex];
            }

            StartStageAsync(stageSetting).Forget();
        }

        public void EndGame()
        {
        }

        public void TryFlipCard(int position)
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

            cardModel.IsFlipped.Value = true;

            if (_flippedCards.Count > 0)
            {
                CardModel previousFlippedCardModel = _flippedCards[^1];
                if (previousFlippedCardModel.Index == cardModel.Index)
                {
                    _flippedCards.Add(cardModel);
                }
                else
                {
                    foreach (CardModel flippedCardModel in _flippedCards)
                    {
                        flippedCardModel.IsFlipped.Value = false;
                    }

                    _flippedCards.Clear();
                }

                StageSetting stageSetting = _localSettings.GameSettings.StageSettings[CurrentStageIndex];
                if (_flippedCards.Count == stageSetting.CardsToMatch)
                {
                    foreach (CardModel flippedCardModel in _flippedCards)
                    {
                        flippedCardModel.IsMatched.Value = true;
                    }

                    _cardsMatched += stageSetting.CardsToMatch;
                    _flippedCards.Clear();

                    if (_cardsMatched == stageSetting.CardsAmount)
                    {
                        _currentGameState.Value = GameState.StageCompleted;
                    }
                }
            }
            else
            {
                _flippedCards.Add(cardModel);
            }
        }

        private async UniTaskVoid StartStageAsync(StageSetting stageSetting)
        {
            _cardsMatched = 0;

            _currentCardModelByPositions.Clear();
            _availableCardTypeIndices.Clear();
            _availableCardIndices.Clear();
            _randomCardIndices.Clear();

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
                    _currentCardModelByPositions.Add(position, cardModel);
                }
            }

            _currentGameState.Value = GameState.Remembering;
            await UniTask.WaitForSeconds(stageSetting.TimeToRememberCardsSeconds);
            _currentGameState.Value = GameState.Matching;
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