// Testing Class for DependencyGraph.cs
// <authors> [Leo Yu] </authors>
// <date> [Jan 31, 2025] </date>

namespace DependencyGraphTests;
using CS3500.DependencyGraph;

/// <summary>
///   This is a test class for DependencyGraphTest and is intended
///   to contain all DependencyGraphTest Unit Tests
/// </summary>
[TestClass]
public class DependencyGraphTests
{
  /// <summary>
  ///   This test repeatedly adds and removes dependencies in a DependencyGraph
  ///   and verifies that the final sets of dependents and dependees match
  ///   what we expect. The aim is to "stress test" the data structure with
  ///   a relatively large number of operation under a
  ///   2-second time constraint.
  /// </summary>
  [TestMethod]
  [Timeout( 2000 )]  // 2 second run time limit
  public void StressTest()
  {
    DependencyGraph dg = new();

    // A bunch of strings to use
    const int SIZE = 200;
    string[] letters = new string[SIZE];
    for ( int i = 0; i < SIZE; i++ )
    {
      letters[i] = string.Empty + ( (char) ( 'a' + i ) );
    }

    // The correct answers
    HashSet<string>[] dependents = new HashSet<string>[SIZE];
    HashSet<string>[] dependees = new HashSet<string>[SIZE];
    for ( int i = 0; i < SIZE; i++ )
    {
      dependents[i] = [];
      dependees[i] = [];
    }

    // Add a bunch of dependencies
    for ( int i = 0; i < SIZE; i++ )
    {
      for ( int j = i + 1; j < SIZE; j++ )
      {
        dg.AddDependency( letters[i], letters[j] );
        dependents[i].Add( letters[j] );
        dependees[j].Add( letters[i] );
      }
    }

    // Remove a bunch of dependencies
    for ( int i = 0; i < SIZE; i++ )
    {
      for ( int j = i + 4; j < SIZE; j += 4 )
      {
        dg.RemoveDependency( letters[i], letters[j] );
        dependents[i].Remove( letters[j] );
        dependees[j].Remove( letters[i] );
      }
    }

    // Add some back
    for ( int i = 0; i < SIZE; i++ )
    {
      for ( int j = i + 1; j < SIZE; j += 2 )
      {
        dg.AddDependency( letters[i], letters[j] );
        dependents[i].Add( letters[j] );
        dependees[j].Add( letters[i] );
      }
    }

    // Remove some more
    for ( int i = 0; i < SIZE; i += 2 )
    {
      for ( int j = i + 3; j < SIZE; j += 3 )
      {
        dg.RemoveDependency( letters[i], letters[j] );
        dependents[i].Remove( letters[j] );
        dependees[j].Remove( letters[i] );
      }
    }

    // Make sure everything is right
    for ( int i = 0; i < SIZE; i++ )
    {
      Assert.IsTrue( dependents[i].SetEquals( new HashSet<string>( dg.GetDependents( letters[i] ) ) ) );
      Assert.IsTrue( dependees[i].SetEquals( new HashSet<string>( dg.GetDependees( letters[i] ) ) ) );
    }
  }
  
  // ------ HasDependees() & HasDependants() ------

  [TestMethod]
  public void HasDependees_ExistInmap_Valid()
  {
    DependencyGraph dg = new(); 
    dg.AddDependency("B", "A"); 
    dg.AddDependency("C", "A"); 
    

    Assert.IsTrue(dg.HasDependees("A")); 
  }
  
  [TestMethod]
  public void HasDependees_DoNotExistInmap_Invalid()
  {
    DependencyGraph dg = new();
    dg.AddDependency( "B", "A" );
    dg.AddDependency( "C", "A" );
    
    Assert.IsFalse(dg.HasDependees("D"));
    Assert.IsFalse(dg.HasDependees("E"));
  }
  
  [TestMethod]
  public void HasDependents_ExistInmap_Valid()
  {
    DependencyGraph dg = new();
    dg.AddDependency("A", "C"); 
    dg.AddDependency("A", "B"); 
    
    
    Assert.IsTrue(dg.HasDependents("A")); 
  }
  
  [TestMethod]
  public void HasDependents_DoNotExistInmap_Invalid()
  {
    DependencyGraph dg = new();
    dg.AddDependency( "A", "B" );
    dg.AddDependency( "A", "C" );
    
    Assert.IsFalse(dg.HasDependents("D"));
    Assert.IsFalse(dg.HasDependents("E"));
  }
  
  // ------ GetDependents() & GetDependees() ------

  [TestMethod]
  public void GetDependents_ExisitsInMap_Valid()
  {
    DependencyGraph dg = new();
    dg.AddDependency( "A", "B" );
    dg.AddDependency( "A", "C" );

    HashSet<string> expected = new HashSet<string>();
    expected.Add("B");
    expected.Add("C");
    
    HashSet<string> result = new HashSet<string>(dg.GetDependents("A"));
    
    Assert.IsTrue(result.SetEquals(expected));
  }
  
  [TestMethod]
  public void GetDependents_DoesntExisitInMapEmptyHashSet_Invalid()
  {
    DependencyGraph dg = new();
    dg.AddDependency( "A", "B" );
    dg.AddDependency( "A", "C" );
    
    HashSet<string> result = new HashSet<string>(dg.GetDependents("G"));
    HashSet<string> expected = new HashSet<string>();
    
    Assert.IsTrue(result.SetEquals(expected));
  }

  
  [TestMethod]
  public void GetDependees_ExisitsInMap_Valid()
  {
    DependencyGraph dg = new();
    dg.AddDependency( "B", "A" );
    dg.AddDependency( "C", "A" );

    HashSet<string> expected = new HashSet<string> { "B", "C" };
    HashSet<string> result = new HashSet<string>(dg.GetDependees("A")); 
    Assert.IsTrue(result.SetEquals(expected));
  }
  
  [TestMethod]
  public void GetDependees_DoesntExisitInMapEmptyHashSet_Invalid()
  {
    DependencyGraph dg = new();
    dg.AddDependency( "B", "A" );
    dg.AddDependency( "C","A" );
    
    HashSet<string> result = new HashSet<string>(dg.GetDependees("G"));
    HashSet<string> expected = new HashSet<string>();
    
    Assert.IsTrue(result.SetEquals(expected));
  }
  
  // --- AddDependency() & RemoverDependency()

  [TestMethod]
  public void AddDependency_AddNewDependee_Valid()
  {
    DependencyGraph dg = new();
    dg.AddDependency( "B", "A" );
    dg.AddDependency( "C", "A" );
    dg.AddDependency("D", "A");
    
    Assert.IsTrue(dg.HasDependees("A"));
  }

  [TestMethod]
  public void AddDependency_AddNewDependents_Valid()
  {
    DependencyGraph dg = new();
    dg.AddDependency("A", "B");
    dg.AddDependency("A", "C");
    dg.AddDependency("A", "D");

    Assert.IsTrue(dg.HasDependents("A"));
  }
  
  [TestMethod]
  public void AddDependency_AddNewDependeeDuplicate_Valid()
  {
    DependencyGraph dg = new();
    dg.AddDependency( "B", "A" );
    dg.AddDependency( "C", "A" );
    dg.AddDependency("D", "A");
    
    int beforeDupe = dg.Size;
    Assert.AreEqual(3, beforeDupe);
    
    dg.AddDependency( "B", "A" );
    int afterDupe = dg.Size;
    Assert.AreEqual(3, afterDupe);
  }
  
  // ------ RemoveDependent() $ RemoveDependee() ------
  
  [TestMethod]
  public void AddDependency_removeADependee_Valid()
  {
    DependencyGraph dg = new();
    dg.AddDependency( "B", "A" );
    dg.AddDependency( "C", "A" );
    dg.AddDependency("D", "A");
    
    dg.RemoveDependency("D", "A");
    
    Assert.IsFalse(dg.HasDependees("D"));
  }
  
  [TestMethod]
  public void AddDependency_RemoveADependents_Valid()
  {
    DependencyGraph dg = new();
    dg.AddDependency("A", "B");
    dg.AddDependency("A", "C");
    dg.AddDependency("A", "D");
    
    dg.RemoveDependency("A", "D");

    Assert.IsFalse(dg.HasDependents("D"));
  }

  [TestMethod]
  public void RemoveDependency_RemoveNonExistentDependent_Valid()
  {
    DependencyGraph dg = new();
    dg.AddDependency("A", "B");
    dg.AddDependency("A", "C");
    dg.AddDependency("A", "D");
    
    int sizeBeforeRemove = dg.Size;
    Assert.AreEqual(3, sizeBeforeRemove);
    
    dg.RemoveDependency("A", "T");
    int sizeAfterRemove = dg.Size;
    Assert.AreEqual(3, sizeAfterRemove);
  }

  // ------ ReplaceDependents() $ ReplaceDependees() ------

  [TestMethod]
  public void ReplaceDependents_ReplaceDependentsSet_Valid()
  {
    DependencyGraph dg = new();
    dg.AddDependency("A", "B");
    dg.AddDependency("A", "C");
    dg.AddDependency("A", "D");
      
    HashSet<string> expected = new HashSet<string>();
    expected.Add("X");
    expected.Add("Y");
    expected.Add("Z");
    
    dg.ReplaceDependents("A", expected);

    HashSet<string> result = new HashSet<string>(dg.GetDependents("A"));
    
    Assert.IsTrue(result.SetEquals(expected));

  }
  
  [TestMethod]
  public void ReplaceDependees_ReplaceDependeesSet_Valid()
  {
    DependencyGraph dg = new();
    dg.AddDependency( "B", "A" );
    dg.AddDependency( "C", "A" );
    dg.AddDependency("D", "A");
      
    HashSet<string> expected = new HashSet<string>();
    expected.Add("X");
    expected.Add("Y");
    expected.Add("Z");
    
    dg.ReplaceDependees("A", expected);

    HashSet<string> result = new HashSet<string>(dg.GetDependees("A"));
    
    Assert.IsTrue(result.SetEquals(expected));

  }

  [TestMethod]
  public void ReplaceDependees_ReplaceEmptyDependeeSet_Valid()
  {
    DependencyGraph dg = new();
    dg.AddDependency( "A", "B" );
    dg.AddDependency( "A", "C" );
    dg.AddDependency("A", "C");
      
    HashSet<string> expected = new HashSet<string>();
    expected.Add("X");
    expected.Add("Y");
    expected.Add("Z");
    
    dg.ReplaceDependees("A", expected);
    
    HashSet<string> result = new HashSet<string>(dg.GetDependees("A"));

    Assert.IsTrue(result.SetEquals(expected));
  }

  // ------ Size ------

  [TestMethod]
  public void Size_EmptyGraph_Valid()
  {
    DependencyGraph dg = new();
    int result = dg.Size;
    Assert.AreEqual(0, result);
  }

  [TestMethod]
  public void Size_GraphContainsOneBiDirectionalRelationship_Valid()
  {
    DependencyGraph dg = new();
    dg.AddDependency("A", "B");
    int result = dg.Size;
    Assert.AreEqual(1, result);
  }

  [TestMethod]
  public void Size_GraphUpdatesCorrectSizeAfterDelete_Valid()
  {
    DependencyGraph dg = new();
    dg.AddDependency("A", "B");
    dg.AddDependency("C", "D");
    int result = dg.Size;
    Assert.AreEqual(2, result);
    
    dg.RemoveDependency("A", "B");
    int result2 = dg.Size;
    Assert.AreEqual(1, result2);
  }
  
  [TestMethod]
  public void Size_GraphUpdatesCorrectSizeAfterAdd_Valid()
  {
    DependencyGraph dg = new();
    dg.AddDependency("A", "B");
    int result = dg.Size;
    Assert.AreEqual(1, result);
    
    dg.AddDependency("C", "D");
    int result2 = dg.Size;
    Assert.AreEqual(2, result2);
  }

}