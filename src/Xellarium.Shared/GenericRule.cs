using System.ComponentModel.DataAnnotations;

namespace Xellarium.Shared;

public class GenericRule
{
    public static GenericRule GameOfLife = new Builder(2)
        // Мертвая клетка с тремя живыми соседями становится живой
        .WithTransition(0, 1, new Dictionary<int, IList<int>> { {1, [3]} })
        // Живая клетка, у которой 2 или 3 живых соседа, выживает
        .WithTransition(1, 1, new Dictionary<int, IList<int>> { {1, [2, 3]} })
        // Иначе погибает
        .WithTransition(1, 0, new Dictionary<int, IList<int>>())
        .Build();

    public static GenericRule WireWorld = new Builder(4)
        // 0 - пусто
        // 1 - сигнал
        // 2 - хвост
        // 3 - проводник
        // Сигнал становится хвостом
        .WithTransition(1, 2, new Dictionary<int, IList<int>>())
        // Хвост становится проводником
        .WithTransition(2, 3, new Dictionary<int, IList<int>>())
        // Проводник становится сигналом, если рядом 1 или 2 сигнала
        .WithTransition(3, 1, new Dictionary<int, IList<int>> { { 1, [1, 2] } })
        .Build();
    
    [Range(2, int.MaxValue)]
    public int StatesCount { get; set; }
    
    public IList<Transition>[] StateTransitions { get; set; }

    public GenericRule(int statesCount)
    {
        if (statesCount < 2)
            throw new ArgumentOutOfRangeException(nameof(statesCount), statesCount, "Minimum state count is 2");
        StatesCount = statesCount;
        StateTransitions = new IList<Transition>[statesCount];
        for (int i = 0; i < statesCount; i++)
        {
            StateTransitions[i] = new List<Transition>();
        }
    }
    
    public GenericRule() : this(2)
    {
    }

    public int GetNextState(int fromState, IReadOnlyDictionary<int, int> neighbours)
    {
        if (fromState < 0 || fromState >= StatesCount)
        {
            return fromState;
        }

        var transitions = StateTransitions[fromState];
        if (transitions.Count == 0)
        {
            return fromState;
        }
        
        foreach (var transition in transitions)
        {
            if (transition.IsSatisfiedBy(neighbours))
            {
                return transition.TargetState;
            }
        }

        return fromState;
    }
    
    public IReadOnlyDictionary<int, int> GetNeighbours(Vec2 pos, World world, IList<Vec2> offsets)
    {
        var result = new DefaultDictionary<int, int>();
        foreach (var offset in offsets)
        {
            result[world[pos + offset]]++;
        }
        return result;
    }

    public int GetNextState(Vec2 pos, World world, IList<Vec2> offsets)
    {
        var neighbours = GetNeighbours(pos, world, offsets);
        var currentState = world[pos];
        var nextState = GetNextState(currentState, neighbours);
        return nextState;
    }

    public World NextState(World w, IList<Vec2> offsets)
    {
        var width = w.Width;
        var height = w.Height;
        var newWorld = new World(width, height);
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                newWorld[i, j] = GetNextState(new Vec2(i, j), w, offsets);
            }
        }
        return newWorld;
    }

    public class Builder
    {
        private bool isBuilt = false;
        private readonly GenericRule _currentGenericRule;

        public Builder(int statesCount)
        {
            _currentGenericRule = new GenericRule(statesCount);
        }

        public Builder WithTransition(int fromState, int toState, Dictionary<int, IList<int>> requiredNeighbours)
        {
            if (isBuilt)
            {
                HelpThrowAlreadyBuiltException();
            }

            if (fromState < 0 || fromState >= _currentGenericRule.StatesCount)
            {
                throw new ArgumentOutOfRangeException(nameof(fromState), fromState,
                    $"Origin state must be from 0 to {_currentGenericRule.StatesCount - 1}");
            }

            var allTransitions = _currentGenericRule.StateTransitions;
            IList<Transition> transitionsList = allTransitions[fromState];
            var transition = new Transition(toState, requiredNeighbours);
            transitionsList.Add(transition);
            return this;
        }
        
        public GenericRule Build()
        {
            if (isBuilt)
            {
                HelpThrowAlreadyBuiltException();
            }
            isBuilt = true;
            return _currentGenericRule;
        }

        private void HelpThrowAlreadyBuiltException()
        {
            throw new InvalidOperationException("Rule is already built with that builder");
        }
    }
}