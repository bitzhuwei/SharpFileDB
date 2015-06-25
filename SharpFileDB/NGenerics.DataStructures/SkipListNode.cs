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

namespace NGenerics.DataStructures
{
	internal class SkipListNode<TKey, TValue>
	{
		#region Globals

		private TKey thisKey;
		private TValue thisValue;
		private SkipListNode<TKey, TValue> rightNode, downNode;

		#endregion

		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="SkipListNode&lt;TKey, TValue&gt;"/> class.
		/// </summary>
		internal SkipListNode()	{}

		/// <summary>
		/// Initializes a new instance of the <see cref="SkipListNode&lt;TKey, TValue&gt;"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="val">The value.</param>
		internal SkipListNode(TKey key, TValue val)
		{
			thisKey = key;
			thisValue = val;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the key.
		/// </summary>
		/// <value>The key.</value>
		internal TKey Key
		{
			get
			{
				return thisKey;
			}
			set
			{
				thisKey = value;
			}
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		internal TValue Value
		{
			get
			{
				return thisValue;
			}
			set
			{
				thisValue = value;
			}
		}

		/// <summary>
		/// Gets or sets the right node.
		/// </summary>
		/// <value>The right node.</value>
		internal SkipListNode<TKey, TValue> Right
		{
			get
			{
				return rightNode;
			}
			set
			{
				rightNode = value;
			}
		}

		

		/// <summary>
		/// Gets or sets the down node.
		/// </summary>
		/// <value>The down node.</value>
		internal SkipListNode<TKey, TValue> Down
		{
			get
			{
				return downNode;
			}
			set
			{
				downNode = value;
			}
		}

		#endregion
	}
}
