// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)
//  Solutions written by Zach Blomquist
//  9/6/22

using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    /**
     * This class represents a Dependency Graph, 
     * which contains what variables are dependent or depending on each other
     */
    public class DependencyGraph
    {
        //The number of pairs within the Dependency Graph
        private int pairCount;
        //A dictionary of values that are depending on values
        private Dictionary<String, List<String>> dependees;
        //A dictionary of values that are being depended on and its dependees
        private Dictionary<String, List<String>> dependents;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependees = new Dictionary<string, List<String>>();
            dependents = new Dictionary<string, List<String>>();
            pairCount = 0;
        }



        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get
            {
                return pairCount;
            }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                return dependents[s].Count;
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (dependees[s].Count is 0)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (this[s] is 0)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            IEnumerable<string> keyList = new List<string>();
            //Checks if s is in dependents, and checks if s has dependees
            if (dependents.ContainsKey(s) && HasDependees(s))
            {
                keyList = new List<string>(this.dependents[s]);
            }
            return keyList;
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            IEnumerable<string> keyList = new List<string>();
            //Checks if s is in dependees and checks if s has dependents
            if (dependees.ContainsKey(s) && HasDependents(s))
            {
                keyList = new List<string>(this.dependees[s]);
            }

            return keyList;
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            //Checks if S is in dependents
            if (dependents.ContainsKey(s))
            {
                //Then checks if t is dependees
                if (!(dependents[s].Contains(t)))
                {
                    //Otherwise adds t to the list at key s and adds t to dependees and s to the list at key t
                    dependents[s].Add(t);
                    //If t does not exist in dependees, adds it to the list
                    if (!(dependees.ContainsKey(t)))
                    {
                        dependees.Add(t, new List<String>());
                    }
                    //Adds s to dependees at t and increases paircount
                    dependees[t].Add(s);
                    pairCount++;
                }

            }
            else
            {
                //If S is not in dependents, adds s to key, and t a new list at key s
                dependents.Add(s, new List<String>());
                dependents[s].Add(t);
                //Checks if t exists in dependees
                if (dependees.ContainsKey(t) && !(dependees[t].Contains(s)))
                {
                    //If it does, add s to list at key t
                    dependees[t].Add(s);
                }
                else
                {
                    //If t is not in dependees, adds t to key, and s to a new list at key t
                    dependees.Add(t, new List<String>());
                    dependees[t].Add(s);
                }
                //Increase pair count
                pairCount++;

            }
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {

            //Checks if pair is contained within dependents and dependees
            if (dependents.ContainsKey(s) && dependees.ContainsKey(t))
            {
                //If so, removes t from key s in dependents, and removes s from key t in dependees
                dependents[s].Remove(t);
                dependees[t].Remove(s);
                //Reduces pairCount
                pairCount--;
            }
            else
            {
                //If pair does not exist, throw exception
                throw new ArgumentException("Pair does not exist");
            }

        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            //Checks if s is contained in dependents
            if (dependents.ContainsKey(s))
            {
                //Removes dependees from dependents in s, and reduces the pair count accordingly
                pairCount -= dependents[s].Count;
                dependents[s].Clear();
                //Adds new dependents to dependent at s
                for (int i = 0; i < newDependents.Count(); i++)
                {
                    AddDependency(s, newDependents.ElementAt(i));
                    dependents[s].Add(newDependents.ElementAt(i));
                }
            }


        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            //Checks if s is contained within dependees
            if (dependees.ContainsKey(s))
            {
                //Removes the pairs count contained within dependees at s
                pairCount -= dependees[s].Count;
                //Clears the dependents at dependees at s
                dependees[s].Clear();
                //Adds the new dependees at dependees at s
                for (int i = 0; i < newDependees.Count(); i++)
                {
                    AddDependency(newDependees.ElementAt(i), s);
                }
            }
        }

    }


}