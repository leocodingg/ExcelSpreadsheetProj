// Skeleton implementation written by Joe Zachary for CS 3500, September 2013
// Version 1.1 - Joe Zachary
//   (Fixed error in comment for RemoveDependency)
// Version 1.2 - Daniel Kopta Fall 2018
//   (Clarified meaning of dependent and dependee)
//   (Clarified names in solution/project structure)
// Version 1.3 - H. James de St. Germain Fall 2024
// <authors> [Leo Yu] </authors>
// <date> [Jan 31, 2025] </date>

namespace CS3500.DependencyGraph;

/// <summary>
///   <para>
///     (s1,t1) is an ordered pair of strings, meaning t1 depends on s1.
///     (in other words: s1 must be evaluated before t1.)
///   </para>
///   <para>
///     A DependencyGraph can be modeled as a set of ordered pairs of strings.
///     Two ordered pairs (s1,t1) and (s2,t2) are considered equal if and only
///     if s1 equals s2 and t1 equals t2.
///   </para>
///   <remarks>
///     Recall that sets never contain duplicates.
///     If an attempt is made to add an element to a set, and the element is already
///     in the set, the set remains unchanged.
///   </remarks>
///   <para>
///     Given a DependencyGraph DG:
///   </para>
///   <list type="number">
///     <item>
///       If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
///       (The set of things that depend on s.)
///     </item>
///     <item>
///       If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
///       (The set of things that s depends on.)
///     </item>
///   </list>
///   <para>
///      For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}.
///   </para>
///   <code>
///     dependents("a") = {"b", "c"}
///     dependents("b") = {"d"}
///     dependents("c") = {}
///     dependents("d") = {"d"}
///     dependees("a")  = {}
///     dependees("b")  = {"a"}
///     dependees("c")  = {"a"}
///     dependees("d")  = {"b", "d"}
///   </code>
/// </summary>
public class DependencyGraph
{
  private Dictionary<string, HashSet<string>> _dependees;
  private Dictionary<string, HashSet<string>> _dependents;
  private int _size;
  
  /// <summary>
  ///   Initializes a new instance of the <see cref="DependencyGraph"/> class.
  ///   The initial DependencyGraph is empty.
  /// </summary>
  public DependencyGraph()
  {
    _dependees = new Dictionary<string, HashSet<string>>();
    _dependents = new Dictionary<string, HashSet<string>>();
    _size = 0;
  }

  /// <summary>
  /// The number of ordered pairs in the DependencyGraph.
  /// </summary>
  public int Size
  {
    get { return _size; }
  }

  /// <summary>
  ///   Reports whether the given node has dependents (i.e., other nodes depend on it).
  /// </summary>
  /// <param name="nodeName"> The name of the node.</param>
  /// <returns> true if the node has dependents. </returns>
  public bool HasDependents(string nodeName)
  {
    return _dependents.ContainsKey(nodeName);
  }

  /// <summary>
  ///   Reports whether the given node has dependees (i.e., depends on one or more other nodes).
  /// </summary>
  /// <returns> true if the node has dependees.</returns>
  /// <param name="nodeName">The name of the node.</param>
  public bool HasDependees(string nodeName)
  {
    return _dependees.ContainsKey(nodeName);
  }

  /// <summary>
  ///   <para>
  ///     Returns the dependents of the node with the given name.
  ///   </para>
  /// </summary>
  /// <param name="nodeName"> The node we are looking at.</param>
  /// <returns> The dependents of nodeName. </returns>
  public IEnumerable<string> GetDependents( string nodeName )
  {
    return _dependents.TryGetValue(nodeName, out var dependents) 
      ? new HashSet<string>(dependents) : new HashSet<string>();
  }

  /// <summary>
  ///   <para>
  ///     Returns the dependees of the node with the given name.
  ///   </para>
  /// </summary>
  /// <param name="nodeName"> The node we are looking at.</param>
  /// <returns> The dependees of nodeName. </returns>
  public IEnumerable<string> GetDependees( string nodeName )
  {
    return _dependees.TryGetValue(nodeName, out var dependees) 
      ? new HashSet<string>(dependees) : new HashSet<string>();
  }

  /// <summary>
  /// <para>Adds the ordered pair (dependee, dependent), if it doesn't exist.</para>
  ///
  /// <para>
  ///   This can be thought of as: dependee must be evaluated before dependent
  /// </para>
  /// </summary>
  /// <param name="dependee"> the name of the node that must be evaluated first</param>
  /// <param name="dependent"> the name of the node that cannot be evaluated until after dependee</param>
  public void AddDependency(string dependee, string dependent)
  {
    if (AddDependent(dependee, dependent))
    {
      if (AddDependee(dependent, dependee))
      {
        _size++;
      }
    }
  }

  /// <summary>
  ///   <para>
  ///     Ensures that the 'dependee' node has an entry in the '_dependents' dictionary, 
  ///     then adds 'dependent' to that set if it's not already present.
  ///   </para>
  /// </summary>
  /// <param name="dependee">
  ///   The node on which 'dependent' depends (must be evaluated before 'dependent').
  /// </param>
  /// <param name="dependent">
  ///   The node that depends on 'dependee'.
  /// </param>
  /// <returns>
  ///   <see langword="true"/> if 'dependent' was newly added to 'dependee's set of dependents;
  ///   otherwise, <see langword="false"/> if it was already present.
  /// </returns>
  private bool AddDependent(string dependee, string dependent)
  {
    // Ensure the dependee has an entry in the dependents dictionary
    if (!_dependents.TryGetValue(dependee, out var dependentsOfDependee))
    {
      dependentsOfDependee = new HashSet<string>();
      _dependents[dependee] = dependentsOfDependee;
    }

    return dependentsOfDependee.Add(dependent);
  }

  /// <summary>
  ///   <para>
  ///     Ensures that the 'dependent' node has an entry in the '_dependees' dictionary,
  ///     then adds 'dependee' to that set if it's not already present.
  ///   </para>
  /// </summary>
  /// <param name="dependent">
  ///   The node that depends on 'dependee'.
  /// </param>
  /// <param name="dependee">
  ///   The node that must be evaluated before 'dependent'.
  /// </param>
  /// <returns>
  ///   <see langword="true"/> if 'dependee' was newly added to 'dependent's set of dependees;
  ///   otherwise, <see langword="false"/> if it was already present.
  /// </returns>
  private bool AddDependee(string dependent, string dependee)
  {
    // Ensure the dependent has an entry in the dependees dictionary
    if (!_dependees.TryGetValue(dependent, out var dependeesOfDependent))
    {
      dependeesOfDependent = new HashSet<string>();
      _dependees[dependent] = dependeesOfDependent;
    }

    return dependeesOfDependent.Add(dependee);
  }


  /// <summary>
  ///   <para>
  ///     Removes the ordered pair (dependee, dependent), if it exists.
  ///   </para>
  /// </summary>
  /// <param name="dependee"> The name of the node that must be evaluated first</param>
  /// <param name="dependent"> The name of the node that cannot be evaluated until after dependee</param>
  public void RemoveDependency( string dependee, string dependent )
  {
    if (RemoveDependent(dependee, dependent))
    {
      RemoveDependee(dependent, dependee);

      _size--;
    }
  }

  /// <summary>
  ///   <para>
  ///     Removes 'dependent' from the 'dependee' node's set of dependents if present. 
  ///     If that set becomes empty, the 'dependee' key is removed from '_dependents'.
  ///   </para>
  /// </summary>
  /// <param name="dependee">
  ///   The node on which 'dependent' depends.
  /// </param>
  /// <param name="dependent">
  ///   The node that depends on 'dependee'.
  /// </param>
  /// <returns>
  ///   <see langword="true"/> if 'dependent' was successfully removed;
  ///   otherwise, <see langword="false"/> if it wasn't present.
  /// </returns>
  private bool RemoveDependent(string dependee, string dependent)
  {
    if (_dependents.TryGetValue(dependee, out var dependentsOfDependee))
    {
      if (dependentsOfDependee.Remove(dependent))
      {
        if (dependentsOfDependee.Count == 0)
        {
          _dependents.Remove(dependee);
        }
        return true;
      }
    }
    return false;
  }
  
  /// <summary>
  ///   <para>
  ///     Removes 'dependee' from the 'dependent' node's set of dependees if present. 
  ///     If that set becomes empty, the 'dependent' key is removed from '_dependees'.
  ///   </para>
  /// </summary>
  /// <param name="dependent">
  ///   The node that depends on 'dependee'.
  /// </param>
  /// <param name="dependee">
  ///   The node that must be evaluated before 'dependent'.
  /// </param>
  private void RemoveDependee(string dependent, string dependee)
  {
    if (_dependees.TryGetValue(dependent, out var dependeesOfDependent))
    {
      dependeesOfDependent.Remove(dependee);

      if (dependeesOfDependent.Count == 0)
      {
        _dependees.Remove(dependent);
      }
    }
  }

  /// <summary>
  ///   Removes all existing ordered pairs of the form (nodeName, *).  Then, for each
  ///   t in newDependents, adds the ordered pair (nodeName, t).
  /// </summary>
  /// <param name="nodeName"> The name of the node who's dependents are being replaced </param>
  /// <param name="newDependents"> The new dependents for nodeName</param>
  public void ReplaceDependents(string nodeName, IEnumerable<string> newDependents)
  {
    // Remove current dependents
    if (_dependents.TryGetValue(nodeName, out var dependents))
    {
      foreach (var t in dependents.ToList())
      {
        RemoveDependency(nodeName, t); 
      }
    }
    // Add new dependents
    foreach (var t in newDependents)
    {
      AddDependency(nodeName, t);
    }
  }
  
  /// <summary>
  ///   <para>
  ///     Removes all existing ordered pairs of the form (*, nodeName).  Then, for each
  ///     t in newDependees, adds the ordered pair (t, nodeName).
  ///   </para>
  /// </summary>
  /// <param name="nodeName"> The name of the node who's dependees are being replaced</param>
  /// <param name="newDependees"> The new dependees for nodeName</param>
  public void ReplaceDependees(string nodeName, IEnumerable<string> newDependees)
  {
    // Remove current dependees
    if (_dependees.TryGetValue(nodeName, out var dependees))
    {
      foreach (var s in dependees.ToList())
      {
        RemoveDependency(s, nodeName);
      }
    }

    // Add new dependees
    foreach (var s in newDependees)
    {
      AddDependency(s, nodeName);
    }
  }
}