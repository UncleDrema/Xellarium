using System.Collections.Immutable;

namespace Xellarium.Shared;

public class Transition
{
    public int TargetState { get; set; }
    public IDictionary<int, IList<int>> RequiredNeighbours { get; set; }

    public Transition()
    {
        
    }
    
    public Transition(int targetState, IDictionary<int, IList<int>> requiredNeighbours)
    {
        TargetState = targetState;
        RequiredNeighbours = requiredNeighbours;
    }
    
    public Transition(int targetState) : this(targetState, ImmutableDictionary<int, IList<int>>.Empty) { }

    public bool IsSatisfiedBy(IReadOnlyDictionary<int, int> neighbours)
    {
        if (RequiredNeighbours.Count == 0)
            return true;
        
        foreach (var (state, requirement) in RequiredNeighbours)
        {
            if (neighbours.TryGetValue(state, out int amount) &&
                requirement.Contains(amount))
            {
                return true;
            }
        }

        return false;
    }
}