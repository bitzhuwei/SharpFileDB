/*  
 Author: Riaan Hanekom

 Copyright 2007 Riaan Hanekom

 Permission is hereby granted, free of charge, to any person obtaining
 a copy of this software and associated documentation files (the
 "Software"), to deal in the Software without restriction, including
 without limitation the rights to use, copy, modify, merge, publish,
 distribute, sublicense, and/or sell copies of the Software, and to
 permit persons to whom the Software is furnished to do so, subject to
 the following conditions:

 The above copyright notice and this permission notice shall be
 included in all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
 LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
 OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
 WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using NGenerics.Visitors;
//using NGenerics.Misc;

namespace NGenerics.DataStructures
{
    /// <summary>
    /// A Skip List data structure
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
	public class SkipList<TKey, TValue> : IVisitableDictionary<TKey, TValue>
	{
		#region Globals

		private IComparer<TKey> comparerToUse;
		private int maxLevelToUse;
		private double probabilityToUse;
		private int currentLevel;
		private int itemsCount;
		private SkipListNode<TKey, TValue>[] headNodes;
		private SkipListNode<TKey, TValue> tail = new SkipListNode<TKey, TValue>();
		
		// Initialise the random number generator with the current time.
		private Random rand = new Random(System.Convert.ToInt32(DateTime.Now.Ticks % Int32.MaxValue));

		#endregion

		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="SkipList&lt;TKey, TValue&gt;"/> class.
		/// </summary>
		public SkipList() : this(16, 0.5, Comparer<TKey>.Default) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SkipList&lt;TKey, TValue&gt;"/> class.
		/// </summary>
		/// <param name="comparer">The comparer.</param>
		public SkipList(IComparer<TKey> comparer) : this(16, 0.5, comparer) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="SkipList&lt;TKey, TValue&gt;"/> class.
		/// </summary>
		/// <param name="maxLevel">The max level.</param>
		/// <param name="probability">The probability.</param>
		/// <param name="comparer">The comparer.</param>
		public SkipList(int maxLevel, double probability, IComparer<TKey> comparer)
		{
			if (maxLevel < 1)
			{
                //throw new ArgumentOutOfRangeException("maxLevel", maxLevel, Resources.MaximumLevelBiggerThan0);
				throw new ArgumentOutOfRangeException("maxLevel");
			}

			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			if ((probability > 0.9) || (probability < 0.1))
			{
                //throw new ArgumentOutOfRangeException("probability", probability, Resources.InvalidProbability);
                throw new ArgumentOutOfRangeException("probability"); 
			}

			comparerToUse = comparer;
			maxLevelToUse = maxLevel;
			probabilityToUse = probability;
			
			// Initialise the skip list to empty nodes, and link the heads and the tails
			headNodes = new SkipListNode<TKey, TValue>[maxLevel];

			headNodes[0] = new SkipListNode<TKey, TValue>();
			headNodes[0].Right = tail;

			for (int i = 1; i < maxLevel; i++)
			{
				headNodes[i] = new SkipListNode<TKey, TValue>();
				headNodes[i].Down = headNodes[i - 1];
				//headNodes[i - 1].Up = headNodes[i];
				headNodes[i].Right = tail;
			}
		}

		#endregion

		#region IVisitableCollection<T> Members

		/// <summary>
		/// Accepts the specified visitor.
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		public void Accept(IVisitor<KeyValuePair<TKey, TValue>> visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}

			// Start at the bottom level and add all the keys to the return array.
			SkipListNode<TKey, TValue> startNode = headNodes[0];

			for (int i = 0; i < this.Count; i++)
			{
				startNode = startNode.Right;
				visitor.Visit(new KeyValuePair<TKey, TValue>(startNode.Key, startNode.Value));

				if (visitor.HasCompleted)
				{
					break;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is of a fixed size.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is fixed size; otherwise, <c>false</c>.
		/// </value>
		public bool IsFixedSize {
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this collection is empty.
		/// </summary>
		/// <value><c>true</c> if this collection is empty; otherwise, <c>false</c>.</value>
		public bool IsEmpty {
			get
			{
				return this.Count == 0;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this collection is full.
		/// </summary>
		/// <value><c>true</c> if this collection is full; otherwise, <c>false</c>.</value>
		public bool IsFull {
			get
			{
				return false;
			}
		}			

		#endregion

		#region ICollection<T> Members

		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public int Count
		{
			get
			{
				return itemsCount;
			}
		}


		/// <summary>
		/// Gets a value indicating whether this instance is read only.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
		/// </value>
		public bool IsReadOnly {
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Adds the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Add(KeyValuePair<TKey, TValue> item)
		{
			this.Add(item.Key, item.Value);
		}

		/// <summary>
		/// Clears this instance.
		/// </summary>
		public void Clear()
		{
			// Set all the heads' references to the tail to eliminate the items in between
			for (int i = 0; i < maxLevelToUse; i++)
			{
				headNodes[i].Right = tail;
			}

			itemsCount = 0;
		}

		/// <summary>
		/// Determines whether [contains] [the specified item].
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.
		/// </returns>		
		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			SkipListNode<TKey, TValue> node = Find(item.Key);

			if (node == null)
			{
				return false;
			}
			else
			{
				if (node.Value.Equals(item.Value))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Copies to.
		/// </summary>
		/// <param name="array">The array.</param>
		/// <param name="arrayIndex">Index of the array.</param>
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}

			if ((array.Length - arrayIndex) < this.Count)
			{
                //throw new ArgumentException(Resources.NotEnoughSpaceInTargetArray);
                throw new ArgumentException("NotEnoughSpaceInTargetArray");
			}

			using (IEnumerator<KeyValuePair<TKey, TValue>> enumerator = this.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					array.SetValue(enumerator.Current, arrayIndex++);
				}
			}
		}

		/// <summary>
		/// Removes the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>A value indicating whether the key was found in the tree.</returns>
		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return Remove(item.Key);
		}

		#endregion

		#region IDictionary<T> Members


		/// <summary>
		/// Gets the keys.
		/// </summary>
		/// <value>The keys.</value>
		public ICollection<TKey> Keys {
			get
			{
				// Start at the bottom level and add all the keys to the return array.
				SkipListNode<TKey, TValue> startNode = headNodes[0];
				TKey[] keys = new TKey[this.Count];

				for (int i = 0; i< this.Count; i++) {
					startNode = startNode.Right;
					keys[i] = startNode.Key;
				}

				return keys;
			}
		}

		/// <summary>
		/// Gets the values.
		/// </summary>
		/// <value>The values.</value>
		public ICollection<TValue> Values
		{
			get
			{
				// Start at the bottom level and add all the values to the return array.
				SkipListNode<TKey, TValue> startNode = headNodes[0];
				TValue[] values = new TValue[this.Count];

				for (int i = 0; i < this.Count; i++)
				{
					startNode = startNode.Right;
					values[i] = startNode.Value;
				}

				return values;
			}
		}

		/// <summary>
		/// Gets or sets the value in the node with the specified key.
		/// </summary>
		/// <value></value>
		public TValue this[TKey key] {
			get
			{
				SkipListNode<TKey, TValue> node = Find(key);

				if (node == null)
				{
                    //throw new ArgumentOutOfRangeException(Resources.KeyDoesNotExist);
                    throw new ArgumentOutOfRangeException("KeyDoesNotExist");
				}
				else
				{
					return node.Value;
				}
			}
			set
			{
				SkipListNode<TKey, TValue> node = Find(key);

				if (node == null)
				{
                    //throw new ArgumentOutOfRangeException(Resources.KeyDoesNotExist);
                    throw new ArgumentOutOfRangeException("KeyDoesNotExist");
				}
				else
				{
					node.Value = value;
				}
			}
		}

		/// <summary>
		/// Adds the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public void Add(TKey key, TValue value)
		{
			SkipListNode<TKey, TValue>[] rightNodes = FindRightMostNodes(key);
			
			// Check if the item allready exists in the list.  If it does, throw an exception -
			// we will not allow duplicate items here.
			if ((rightNodes[0].Right != tail) && (comparerToUse.Compare(rightNodes[0].Right.Key, key) == 0))
			{
                //throw new ArgumentException(Resources.ItemAlreadyInList);
                throw new ArgumentException("ItemAlreadyInList");
			}
			else
			{
				int newLevel = PickRandomLevel();
				
				if (newLevel > currentLevel)
				{
					for (int i = currentLevel + 1; i <= newLevel; i++)
					{
						rightNodes[i] = headNodes[i];
					}

					currentLevel = newLevel;
				}

				SkipListNode<TKey, TValue> previousNode = null;
				SkipListNode<TKey, TValue> newNode = new SkipListNode<TKey, TValue>(key, value); ;

				// Insert the item in the first level
				newNode.Right = rightNodes[0].Right;
				rightNodes[0].Right = newNode;

				// And now insert the node in the rest of the levels, making sure
				// to update the the links
				for (int i = 1; i <= currentLevel; i++)
				{
					previousNode = newNode;
					newNode = new SkipListNode<TKey, TValue>(key, value);

					newNode.Right = rightNodes[i].Right;
					rightNodes[i].Right = newNode;
					
					newNode.Down = previousNode;
				}
			}

			itemsCount++;
		}		

		/// <summary>
		/// Determines whether the specified key contains key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// 	<c>true</c> if the specified key contains key; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsKey(TKey key)
		{
			return Find(key) != null;
		}

		/// <summary>
		/// Removes the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>A value indicating whether the key was found (and removed) in the tree.</returns>
		public bool Remove(TKey key)
		{
			SkipListNode<TKey, TValue>[] rightNodes = FindRightMostNodes(key);

			// See if we actually found the node
			if ((rightNodes[0].Right != tail) && (comparerToUse.Compare(rightNodes[0].Right.Key, key) == 0))
			{
				for (int i = 0; i < currentLevel; i++)
				{
					// Since the node is consecutive levels, as soon as we don't find it on the next
					// level, we can stop.
					if ((rightNodes[i].Right != tail) && (comparerToUse.Compare(rightNodes[i].Right.Key, key) == 0))
					{
						rightNodes[i].Right = rightNodes[i].Right.Right;
					}
					else
					{
						break;
					}
				}

				itemsCount--;
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Tries to get the value with the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns>A value indiciating whether the node with the specified key was found in the tree.</returns>
		public bool TryGetValue(TKey key, out TValue value)
		{
			SkipListNode<TKey, TValue> node = Find(key);

			if (node == null)
			{
				value = default(TValue);
				return false;
			}
			else
			{
				value = node.Value;
				return true;
			}
		}

		#endregion

		#region IEnumerable<T> Members

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>An enumerator for enumerating though the collection.</returns>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			// Start at the bottom level and add all the keys to the return array.
			SkipListNode<TKey, TValue> startNode = headNodes[0];

			while (startNode.Right != tail)
			{
				startNode = startNode.Right;
				yield return new KeyValuePair<TKey, TValue>(startNode.Key, startNode.Value);
			}
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>An enumerator for enumerating though the colleciton.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		#region IComparable Members

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than obj. Zero This instance is equal to obj. Greater than zero This instance is greater than obj.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">obj is not the same type as this instance. </exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}

			if (obj.GetType() == this.GetType())
			{
				SkipList<TKey, TValue> s = obj as SkipList<TKey, TValue>;
				return this.Count.CompareTo(s.Count);
			}
			else
			{
				return this.GetType().FullName.CompareTo(obj.GetType().FullName);
			}
		}

		#endregion

		#region Public Members
		
		/// <summary>
		/// Gets the comparer used to compare items in this instance.
		/// </summary>
		/// <value>The comparer.</value>
		public IComparer<TKey> Comparer
		{
			get
			{
				return comparerToUse;
			}
		}

		/// <summary>
		/// Gets the probability.
		/// </summary>
		/// <value>The probability.</value>
		public double Probability
		{
			get
			{
				return probabilityToUse;
			}
		}

		/// <summary>
		/// Gets the max level.
		/// </summary>
		/// <value>The max level.</value>
		public int MaxListLevel
		{
			get
			{
				return maxLevelToUse;
			}
		}

		/// <summary>
		/// Gets the current list level.
		/// </summary>
		/// <value>The current list level.</value>
		public int CurrentListLevel
		{
			get
			{
				return currentLevel;
			}
		}

		#endregion

		#region Private Members

		private SkipListNode<TKey, TValue> Find(TKey key)
		{
			if (this.Count == 0)
			{
				return null;
			}
			else
			{
				// Start at the top list header node
				SkipListNode<TKey, TValue> currentNode = headNodes[currentLevel];

				while (true)
				{
					while ((currentNode.Right != tail) && (comparerToUse.Compare(currentNode.Right.Key, key) < 0))
					{
						currentNode = currentNode.Right;
					}

					// Check if there is a next level, and if there is move down.
					if (currentNode.Down == null)
					{
						break;
					}
					else
					{
						currentNode = currentNode.Down;
					}
				}
				
				// Do one final comparison to see if the key to the right equals this key.
				// If it doesn't match, it would be bigger than this key.
				if (comparerToUse.Compare(currentNode.Right.Key, key) == 0)
				{
					return currentNode.Right;
				}
				else
				{
					return null;
				}
			}
		}

		private int PickRandomLevel()
		{
			int randomLevel = 0;

			while ((rand.NextDouble() < probabilityToUse) && (randomLevel <= currentLevel + 1) && (randomLevel < maxLevelToUse))
			{
				randomLevel++;
			}

			return randomLevel;
		}

		private SkipListNode<TKey, TValue>[] FindRightMostNodes(TKey key)
		{
			SkipListNode<TKey, TValue>[] rightNodes = new SkipListNode<TKey, TValue>[maxLevelToUse];

			// Start at the top list header node
			SkipListNode<TKey, TValue> currentNode = headNodes[currentLevel];

			for (int i = currentLevel; i >= 0; i--)
			{
				while ((currentNode.Right != tail) && (comparerToUse.Compare(currentNode.Right.Key, key) < 0))
				{
					currentNode = currentNode.Right;
				}

				// Store this node - the new node will be to the right of it.
				rightNodes[i] = currentNode;

				// Check if there is a next level, and if there is move down.
				if (i > 0)
				{
					currentNode = currentNode.Down;
				}
			}
			return rightNodes;
		}

		#endregion
	}
}
