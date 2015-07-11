# SharpFileDB
SharpFileDB is a micro database library that uses no SQL, files as storage form and totally C# to implement a CRUD system. 
SharpFileDB是一个纯C#的无SQL的支持CRUD的小型文件数据库，目标是支持万人级别的应用程序。
<p style="text-align: center;"><span style="font-size: 16pt;"><strong>小型单文件NoSQL数据库SharpFileDB初步实现 </strong></span></p>
<p>我不是数据库方面的专家，不过还是想做一个小型的数据库，算是一种通过mission impossible进行学习锻炼的方式。我知道这是自不量力，不过还是希望各路大神批评的时候不要人身攻击，谢谢。</p>
<h1>SharpFileDB</h1>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810176895618.png" alt="" /></p>
<p>最近所做的<a href="http://www.cnblogs.com/bitzhuwei/p/SharpFileDB.html">多文件数据库</a>是受（<a href="http://www.cnblogs.com/gaochundong/archive/2013/04/24/csharp_file_database.html">C#实现文件数据库</a>）的启发。后来又发现了（<a href="http://www.litedb.org/">LiteDB</a>），看到了单文件数据库和分页、索引、查询语句等的实现方式，大受启发。不过我仍旧认为LiteDB使用起来有些不顺畅，它的代码组织也不敢完全苟同。所以，我重新设计了一个小型的单文件数据库SharpFileDB：</p>
<p>无需配置服务器。</p>
<p>无需SQL。</p>
<p>100%纯C#开发的一个不到50KB的DLL。</p>
<p>支持事务ACID。</p>
<p>写入失败后可恢复（日志模式）。</p>
<p>可存储任意继承了Table且具有[Serializable]特性的类型（相当于关系数据库的Table）。类型数目不限。</p>
<p>可存储System.Drawing.Image等大型对象。</p>
<p>单文件存储，只要你的硬盘空间够大，理论上能支持的最大长度为long.MaxValue = 9223372036854775807 = 0x7FFFFFFFFFFFFFFF = 8589934591GB = 8388607TB = 8191PB = 7EB的大文件。</p>
<p>每个类型都可以建立多个索引，索引数目不限。只需在属性上加[TableIndex]特性即可实现。</p>
<p>支持通过Lambda表达式进行查询。</p>
<p>开源免费，2300行代码，1000行注释。</p>
<p>附带Demo、可视化的监视工具、可视化的数据库设计器，便于学习、调试和应用。</p>
<p>&nbsp;</p>
<h1>使用场景</h1>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810185646717.png" alt="" /></p>
<p>假设已经做好了这样一个单文件数据库，我期望的使用方式是这样的：</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span>             <span style="color: #0000ff;">string</span> fullname = Path.Combine(Environment.CurrentDirectory, <span style="color: #800000;">"</span><span style="color: #800000;">test.db</span><span style="color: #800000;">"</span><span style="color: #000000;">);
</span><span style="color: #008080;"> 2</span>             <span style="color: #0000ff;">using</span> (FileDBContext db = <span style="color: #0000ff;">new</span><span style="color: #000000;"> FileDBContext(fullname))
</span><span style="color: #008080;"> 3</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 4</span>                 Cat cat = <span style="color: #0000ff;">new</span><span style="color: #000000;"> Cat();
</span><span style="color: #008080;"> 5</span>                 <span style="color: #0000ff;">string</span> name = <span style="color: #800000;">"</span><span style="color: #800000;">kitty </span><span style="color: #800000;">"</span> +<span style="color: #000000;"> random.Next();
</span><span style="color: #008080;"> 6</span>                 cat.KittyName =<span style="color: #000000;"> name;
</span><span style="color: #008080;"> 7</span>                 cat.Price = random.Next(<span style="color: #800080;">1</span>, <span style="color: #800080;">100</span><span style="color: #000000;">);
</span><span style="color: #008080;"> 8</span> 
<span style="color: #008080;"> 9</span> <span style="color: #000000;">                db.Insert(cat);
</span><span style="color: #008080;">10</span> 
<span style="color: #008080;">11</span>                 System.Linq.Expressions.Expression&lt;Func&lt;Cat, <span style="color: #0000ff;">bool</span>&gt;&gt; pre = <span style="color: #0000ff;">null</span><span style="color: #000000;">;
</span><span style="color: #008080;">12</span> 
<span style="color: #008080;">13</span>                 pre = (x =&gt;
<span style="color: #008080;">14</span>                     (x.KittyName == <span style="color: #800000;">"</span><span style="color: #800000;">kitty</span><span style="color: #800000;">"</span> || (x.KittyName == name &amp;&amp; x.Id.ToString() != <span style="color: #0000ff;">string</span><span style="color: #000000;">.Empty))
</span><span style="color: #008080;">15</span>                     || (x.KittyName.Contains(<span style="color: #800000;">"</span><span style="color: #800000;">kitty</span><span style="color: #800000;">"</span>) &amp;&amp; x.Price &gt; <span style="color: #800080;">10</span><span style="color: #000000;">)
</span><span style="color: #008080;">16</span> <span style="color: #000000;">                    );
</span><span style="color: #008080;">17</span> 
<span style="color: #008080;">18</span>                 IEnumerable&lt;Cat&gt; cats = db.Find&lt;Cat&gt;<span style="color: #000000;">(pre);
</span><span style="color: #008080;">19</span> 
<span style="color: #008080;">20</span>                 cats = db.FindAll&lt;Cat&gt;<span style="color: #000000;">();
</span><span style="color: #008080;">21</span> 
<span style="color: #008080;">22</span>                 cat.KittyName = <span style="color: #800000;">"</span><span style="color: #800000;">小白 </span><span style="color: #800000;">"</span> +<span style="color: #000000;"> random.Next();
</span><span style="color: #008080;">23</span> <span style="color: #000000;">                db.Update(cat);
</span><span style="color: #008080;">24</span> 
<span style="color: #008080;">25</span> <span style="color: #000000;">                db.Delete(cat);
</span><span style="color: #008080;">26</span>             }</pre>
</div>
<p>&nbsp;</p>
<p>就像关系型数据库一样，我们可以创建各种Table（例如这里的Cat）。然后直接使用Insert(Table record);插入一条记录。创建自定义Table只需继承Talbe实现自己的class即可。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 2</span>     <span style="color: #808080;">///</span><span style="color: #008000;"> 继承此类型以实现您需要的Table。
</span><span style="color: #008080;"> 3</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 4</span> <span style="color: #000000;">    [Serializable]
</span><span style="color: #008080;"> 5</span>     <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">abstract</span> <span style="color: #0000ff;">class</span><span style="color: #000000;"> Table : ISerializable
</span><span style="color: #008080;"> 6</span> <span style="color: #000000;">    {
</span><span style="color: #008080;"> 7</span> 
<span style="color: #008080;"> 8</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 9</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 用以区分每个Table的每条记录。
</span><span style="color: #008080;">10</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> This Id is used for diffrentiate instances of 'table's.
</span><span style="color: #008080;">11</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">12</span>         [TableIndex]<span style="color: #008000;">//</span><span style="color: #008000;"> 标记为索引，这是每个表都有的主键。</span>
<span style="color: #008080;">13</span>         <span style="color: #0000ff;">public</span> ObjectId Id { <span style="color: #0000ff;">get</span>; <span style="color: #0000ff;">internal</span> <span style="color: #0000ff;">set</span><span style="color: #000000;">; }
</span><span style="color: #008080;">14</span> 
<span style="color: #008080;">15</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">16</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 创建一个文件对象，在用</span><span style="color: #808080;">&lt;code&gt;</span><span style="color: #008000;">FileDBContext.Insert();</span><span style="color: #808080;">&lt;/code&gt;</span><span style="color: #008000;">将此对象保存到数据库之前，此对象的Id为null。
</span><span style="color: #008080;">17</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">18</span>         <span style="color: #0000ff;">public</span><span style="color: #000000;"> Table() { }
</span><span style="color: #008080;">19</span> 
<span style="color: #008080;">20</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">21</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 显示此条记录的Id。
</span><span style="color: #008080;">22</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">23</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;">24</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">string</span><span style="color: #000000;"> ToString()
</span><span style="color: #008080;">25</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">26</span>             <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">string</span>.Format(<span style="color: #800000;">"</span><span style="color: #800000;">Id: {0}</span><span style="color: #800000;">"</span>, <span style="color: #0000ff;">this</span><span style="color: #000000;">.Id);
</span><span style="color: #008080;">27</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">28</span> 
<span style="color: #008080;">29</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">30</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 使用的字符越少，序列化时占用的字节就越少。一个字符都不用最好。
</span><span style="color: #008080;">31</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">Using less chars means less bytes after serialization. And "" is allowed.</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">32</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">33</span>         <span style="color: #0000ff;">const</span> <span style="color: #0000ff;">string</span> strId = <span style="color: #800000;">""</span><span style="color: #000000;">;
</span><span style="color: #008080;">34</span> 
<span style="color: #008080;">35</span>         <span style="color: #0000ff;">#region</span> ISerializable 成员
<span style="color: #008080;">36</span> 
<span style="color: #008080;">37</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">38</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> This method will be invoked automatically when IFormatter.Serialize() is called.
</span><span style="color: #008080;">39</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">You must use </span><span style="color: #808080;">&lt;code&gt;</span><span style="color: #008000;">base(info, context);</span><span style="color: #808080;">&lt;/code&gt;</span><span style="color: #008000;"> in the derived class to feed </span><span style="color: #808080;">&lt;see cref="Table"/&gt;</span><span style="color: #008000;">'s fields and properties.</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">40</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">当使用IFormatter.Serialize()时会自动调用此方法。</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">41</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">继承此类型时，必须在子类型中用</span><span style="color: #808080;">&lt;code&gt;</span><span style="color: #008000;">base(info, context);</span><span style="color: #808080;">&lt;/code&gt;</span><span style="color: #008000;">来填充</span><span style="color: #808080;">&lt;see cref="Table"/&gt;</span><span style="color: #008000;">自身的数据。</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">42</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">43</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="info"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">44</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="context"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">45</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">virtual</span> <span style="color: #0000ff;">void</span><span style="color: #000000;"> GetObjectData(SerializationInfo info, StreamingContext context)
</span><span style="color: #008080;">46</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">47</span>             <span style="color: #0000ff;">byte</span>[] value = <span style="color: #0000ff;">this</span>.Id.Value;<span style="color: #008000;">//</span><span style="color: #008000;">byte[]比this.Id.ToString()占用的字节少2个字节。</span>
<span style="color: #008080;">48</span> <span style="color: #000000;">            info.AddValue(strId, value);
</span><span style="color: #008080;">49</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">50</span> 
<span style="color: #008080;">51</span>         <span style="color: #0000ff;">#endregion</span>
<span style="color: #008080;">52</span> 
<span style="color: #008080;">53</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">54</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> This method will be invoked automatically when IFormatter.Serialize() is called.
</span><span style="color: #008080;">55</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">You must use </span><span style="color: #808080;">&lt;code&gt;</span><span style="color: #008000;">: base(info, context)</span><span style="color: #808080;">&lt;/code&gt;</span><span style="color: #008000;"> in the derived class to feed </span><span style="color: #808080;">&lt;see cref="Table"/&gt;</span><span style="color: #008000;">'s fields and properties.</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">56</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">当使用IFormatter.Serialize()时会自动调用此方法。</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">57</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">继承此类型时，必须在子类型中用</span><span style="color: #808080;">&lt;code&gt;</span><span style="color: #008000;">: base(info, context)</span><span style="color: #808080;">&lt;/code&gt;</span><span style="color: #008000;">来填充</span><span style="color: #808080;">&lt;see cref="Table"/&gt;</span><span style="color: #008000;">自身的数据。</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">58</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">59</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="info"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">60</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="context"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">61</span>         <span style="color: #0000ff;">protected</span><span style="color: #000000;"> Table(SerializationInfo info, StreamingContext context)
</span><span style="color: #008080;">62</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">63</span>             <span style="color: #0000ff;">byte</span>[] value = (<span style="color: #0000ff;">byte</span>[])info.GetValue(strId, <span style="color: #0000ff;">typeof</span>(<span style="color: #0000ff;">byte</span><span style="color: #000000;">[]));
</span><span style="color: #008080;">64</span>             <span style="color: #0000ff;">this</span>.Id = <span style="color: #0000ff;">new</span><span style="color: #000000;"> ObjectId(value);
</span><span style="color: #008080;">65</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">66</span> 
<span style="color: #008080;">67</span>     }</pre>
</div>
<p>&nbsp;</p>
<p>这里的Cat定义如下：</p>
<p>&nbsp;</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span> <span style="color: #000000;">    [Serializable]
</span><span style="color: #008080;"> 2</span>     <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">class</span><span style="color: #000000;"> Cat : Table
</span><span style="color: #008080;"> 3</span> <span style="color: #000000;">    {
</span><span style="color: #008080;"> 4</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 5</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 显示此对象的信息，便于调试。
</span><span style="color: #008080;"> 6</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 7</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;"> 8</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">string</span><span style="color: #000000;"> ToString()
</span><span style="color: #008080;"> 9</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">10</span>             <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">string</span>.Format(<span style="color: #800000;">"</span><span style="color: #800000;">{0}: ￥{1}</span><span style="color: #800000;">"</span><span style="color: #000000;">, KittyName, Price);
</span><span style="color: #008080;">11</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">12</span> 
<span style="color: #008080;">13</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">string</span> KittyName { <span style="color: #0000ff;">get</span>; <span style="color: #0000ff;">set</span><span style="color: #000000;">; }
</span><span style="color: #008080;">14</span> 
<span style="color: #008080;">15</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">int</span> Price { <span style="color: #0000ff;">get</span>; <span style="color: #0000ff;">set</span><span style="color: #000000;">; }
</span><span style="color: #008080;">16</span> 
<span style="color: #008080;">17</span>         <span style="color: #0000ff;">public</span><span style="color: #000000;"> Cat() { }
</span><span style="color: #008080;">18</span> 
<span style="color: #008080;">19</span>         <span style="color: #0000ff;">const</span> <span style="color: #0000ff;">string</span> strKittyName = <span style="color: #800000;">"</span><span style="color: #800000;">N</span><span style="color: #800000;">"</span><span style="color: #000000;">;
</span><span style="color: #008080;">20</span>         <span style="color: #0000ff;">const</span> <span style="color: #0000ff;">string</span> strPrice = <span style="color: #800000;">"</span><span style="color: #800000;">P</span><span style="color: #800000;">"</span><span style="color: #000000;">;
</span><span style="color: #008080;">21</span> 
<span style="color: #008080;">22</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">void</span><span style="color: #000000;"> GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
</span><span style="color: #008080;">23</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">24</span>             <span style="color: #0000ff;">base</span><span style="color: #000000;">.GetObjectData(info, context);
</span><span style="color: #008080;">25</span> 
<span style="color: #008080;">26</span>             info.AddValue(strKittyName, <span style="color: #0000ff;">this</span><span style="color: #000000;">.KittyName);
</span><span style="color: #008080;">27</span>             info.AddValue(strPrice, <span style="color: #0000ff;">this</span><span style="color: #000000;">.Price);
</span><span style="color: #008080;">28</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">29</span> 
<span style="color: #008080;">30</span>         <span style="color: #0000ff;">protected</span><span style="color: #000000;"> Cat(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
</span><span style="color: #008080;">31</span>             : <span style="color: #0000ff;">base</span><span style="color: #000000;">(info, context)
</span><span style="color: #008080;">32</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">33</span>             <span style="color: #0000ff;">this</span>.KittyName =<span style="color: #000000;"> info.GetString(strKittyName);
</span><span style="color: #008080;">34</span>             <span style="color: #0000ff;">this</span>.Price =<span style="color: #000000;"> info.GetInt32(strPrice);
</span><span style="color: #008080;">35</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">36</span> 
<span style="color: #008080;">37</span>     }</pre>
</div>
<p>&nbsp;</p>
<p>后面我提供了一个可视化的数据库设计器，你可以像在SQL Server Management里那样设计好你需要的表，即可一键生成相应的数据库项目源码。</p>
<p>&nbsp;</p>
<h1>从何开始</h1>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810192991859.png" alt="" /></p>
<p>用C#做一个小型单文件数据库，需要用到.NET Framework提供的这几个类型。</p>
<h2>FileStream</h2>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810199712487.png" alt="" /></p>
<p>文件流用于操作数据库文件。FileStream支持随机读写，并且FileStream.Length属性是long型的，就是说数据库文件最大可以有long.MaxValue个字节，这是超级大的。</p>
<p>使用FileStream的方式是这样的：</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;">1</span> <span style="color: #0000ff;">var</span> fileStream = <span style="color: #0000ff;">new</span> FileStream(fullname, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);</pre>
</div>
<p>&nbsp;</p>
<p>这句代码指明：</p>
<p>fullname：打开绝对路径为fullname的文件。</p>
<p>FileMode.Open：如果文件不存在，抛出异常。</p>
<p>FileAccess.ReadWrite：fileStream对象具有读和写文件的权限。</p>
<p>FileShare.Read：其它进程只能读此文件，不能写。我们可以用其它进程来实现容灾备份之类的操作。</p>
<p>&nbsp;</p>
<h2>BinaryFormatter</h2>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810212524529.png" alt="" /></p>
<p>读写数据库文件实际上就是反序列化和序列化对象的过程。我<a href="http://www.cnblogs.com/bitzhuwei/p/SharpFileDB.html">在这里详细分析了为什么使用BinaryFormatter</a>。</p>
<p>联合使用FileStream和BinaryFormatter就可以实现操作数据库文件的最基础的功能。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 2</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 使用FileStream和BinaryFormatter做单文件数据库的核心工作流。
</span><span style="color: #008080;"> 3</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 4</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="fullname"&gt;&lt;/param&gt;</span>
<span style="color: #008080;"> 5</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">void</span> TypicalScene(<span style="color: #0000ff;">string</span><span style="color: #000000;"> fullname)
</span><span style="color: #008080;"> 6</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 7</span>             <span style="color: #008000;">//</span><span style="color: #008000;"> 初始化。</span>
<span style="color: #008080;"> 8</span>             BinaryFormatter formatter = <span style="color: #0000ff;">new</span><span style="color: #000000;"> BinaryFormatter();
</span><span style="color: #008080;"> 9</span> 
<span style="color: #008080;">10</span>             <span style="color: #008000;">//</span><span style="color: #008000;"> 打开数据库文件。</span>
<span style="color: #008080;">11</span>             FileStream fs = <span style="color: #0000ff;">new</span><span style="color: #000000;"> FileStream(fullname, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
</span><span style="color: #008080;">12</span> 
<span style="color: #008080;">13</span>             <span style="color: #008000;">//</span><span style="color: #008000;"> 把对象写入数据库。</span>
<span style="color: #008080;">14</span>             <span style="color: #0000ff;">long</span> position = <span style="color: #800080;">0</span>;<span style="color: #008000;">//</span><span style="color: #008000;"> 指定位置。</span>
<span style="color: #008080;">15</span> <span style="color: #000000;">            fs.Seek(position, SeekOrigin.Begin);
</span><span style="color: #008080;">16</span>             Object obj = <span style="color: #0000ff;">new</span> Object();<span style="color: #008000;">//</span><span style="color: #008000;"> 此处可以是任意具有[Serializable]特性的类型。</span>
<span style="color: #008080;">17</span>             formatter.Serialize(fs, obj);<span style="color: #008000;">//</span><span style="color: #008000;"> 把对象序列化并写入文件。</span>
<span style="color: #008080;">18</span> 
<span style="color: #008080;">19</span> <span style="color: #000000;">            fs.Flush();
</span><span style="color: #008080;">20</span> 
<span style="color: #008080;">21</span>             <span style="color: #008000;">//</span><span style="color: #008000;"> 从数据库文件读取对象。</span>
<span style="color: #008080;">22</span>             fs.Seek(position, SeekOrigin.Begin);<span style="color: #008000;">//</span><span style="color: #008000;"> 指定位置。</span>
<span style="color: #008080;">23</span>             Object deserialized = formatter.Deserialize(fs);<span style="color: #008000;">//</span><span style="color: #008000;"> 从文件得到反序列化的对象。
</span><span style="color: #008080;">24</span> 
<span style="color: #008080;">25</span>             <span style="color: #008000;">//</span><span style="color: #008000;"> 关闭文件流，退出数据库。</span>
<span style="color: #008080;">26</span> <span style="color: #000000;">            fs.Close();
</span><span style="color: #008080;">27</span> <span style="color: #000000;">            fs.Dispose();
</span><span style="color: #008080;">28</span>         }</pre>
</div>
<p>&nbsp;</p>
<p>简单来说，这就是整个单文件数据库最基本的工作过程。后续的所有设计，目的都在于得到应指定的位置和应读写的对象类型了。能够在合适的位置写入合适的内容，能够通过索引实现快速定位和获取/删除指定的内容，这就是实现单文件数据库要做的第一步。能够实现事务和恢复机制，就是第二步。</p>
<p>&nbsp;</p>
<h2>MemoryStream</h2>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810217689671.png" alt="" /></p>
<p>使用MemoryStream是为了先把对象转换成byte[]，这样就可以计算其序列化后的长度，然后才能为其安排存储到数据库文件的什么地方。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 2</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 把Table的一条记录转换为字节数组。这个字节数组应该保存到Data页。
</span><span style="color: #008080;"> 3</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 4</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="table"&gt;&lt;/param&gt;</span>
<span style="color: #008080;"> 5</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;"> 6</span> <span style="color: #000000;">        [MethodImpl(MethodImplOptions.AggressiveInlining)]
</span><span style="color: #008080;"> 7</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">byte</span>[] ToBytes(<span style="color: #0000ff;">this</span><span style="color: #000000;"> Table table)
</span><span style="color: #008080;"> 8</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 9</span>             <span style="color: #0000ff;">byte</span><span style="color: #000000;">[] result;
</span><span style="color: #008080;">10</span>             <span style="color: #0000ff;">using</span> (MemoryStream ms = <span style="color: #0000ff;">new</span><span style="color: #000000;"> MemoryStream())
</span><span style="color: #008080;">11</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">12</span> <span style="color: #000000;">                Consts.formatter.Serialize(ms, table);
</span><span style="color: #008080;">13</span>                 <span style="color: #0000ff;">if</span> (ms.Length &gt; (<span style="color: #0000ff;">long</span>)<span style="color: #0000ff;">int</span>.MaxValue)<span style="color: #008000;">//</span><span style="color: #008000;"> RULE: 一条记录序列化后最长不能超过int.MaxValue个字节。</span>
<span style="color: #008080;">14</span>                 { <span style="color: #0000ff;">throw</span> <span style="color: #0000ff;">new</span> Exception(<span style="color: #0000ff;">string</span>.Format(<span style="color: #800000;">"</span><span style="color: #800000;">Toooo long is the [{0}]</span><span style="color: #800000;">"</span><span style="color: #000000;">, table)); }
</span><span style="color: #008080;">15</span>                 result = <span style="color: #0000ff;">new</span> <span style="color: #0000ff;">byte</span><span style="color: #000000;">[ms.Length];
</span><span style="color: #008080;">16</span>                 ms.Seek(<span style="color: #800080;">0</span><span style="color: #000000;">, SeekOrigin.Begin);
</span><span style="color: #008080;">17</span>                 ms.Read(result, <span style="color: #800080;">0</span><span style="color: #000000;">, result.Length);
</span><span style="color: #008080;">18</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">19</span> 
<span style="color: #008080;">20</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> result;
</span><span style="color: #008080;">21</span>         }</pre>
</div>
<p>&nbsp;</p>
<p>&nbsp;</p>
<h1>准备知识</h1>
<h2>全局唯一编号</h2>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810223772085.png" alt="" /></p>
<p>写入数据库的每一条记录，都应该有一个全局唯一的编号。（<a href="http://www.cnblogs.com/gaochundong/archive/2013/04/24/csharp_file_database.html">C#实现文件数据库</a>）和（<a href="http://www.litedb.org/">LiteDB</a>）都有一个ObjectId类型，两者也十分相似，且存储它需要的长度也小于.NET Framework自带的Guid，所以就用ObjectId做全局唯一的编号了。</p>
<div class="cnblogs_code" onclick="cnblogs_code_show('a2bd64f7-4f2c-404a-9c52-dca78573c460')"><img id="code_img_closed_a2bd64f7-4f2c-404a-9c52-dca78573c460" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_a2bd64f7-4f2c-404a-9c52-dca78573c460" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('a2bd64f7-4f2c-404a-9c52-dca78573c460',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_a2bd64f7-4f2c-404a-9c52-dca78573c460" class="cnblogs_code_hide">
<pre><span style="color: #008080;">  1</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">  2</span>     <span style="color: #808080;">///</span><span style="color: #008000;"> 用于生成唯一的</span><span style="color: #808080;">&lt;see cref="Table"/&gt;</span><span style="color: #008000;">编号。
</span><span style="color: #008080;">  3</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">  4</span> <span style="color: #000000;">    [Serializable]
</span><span style="color: #008080;">  5</span>     <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">sealed</span> <span style="color: #0000ff;">class</span> ObjectId : ISerializable, IComparable&lt;ObjectId&gt;<span style="color: #000000;">, IComparable
</span><span style="color: #008080;">  6</span> <span style="color: #000000;">    {
</span><span style="color: #008080;">  7</span>         <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">string</span><span style="color: #000000;"> _string;
</span><span style="color: #008080;">  8</span> 
<span style="color: #008080;">  9</span>         <span style="color: #0000ff;">private</span><span style="color: #000000;"> ObjectId()
</span><span style="color: #008080;"> 10</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 11</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 12</span> 
<span style="color: #008080;"> 13</span>         <span style="color: #0000ff;">internal</span> ObjectId(<span style="color: #0000ff;">string</span><span style="color: #000000;"> value)
</span><span style="color: #008080;"> 14</span>             : <span style="color: #0000ff;">this</span><span style="color: #000000;">(DecodeHex(value))
</span><span style="color: #008080;"> 15</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 16</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 17</span> 
<span style="color: #008080;"> 18</span>         <span style="color: #0000ff;">internal</span> ObjectId(<span style="color: #0000ff;">byte</span><span style="color: #000000;">[] value)
</span><span style="color: #008080;"> 19</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 20</span>             Value =<span style="color: #000000;"> value;
</span><span style="color: #008080;"> 21</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 22</span> 
<span style="color: #008080;"> 23</span>         <span style="color: #0000ff;">internal</span> <span style="color: #0000ff;">static</span><span style="color: #000000;"> ObjectId Empty
</span><span style="color: #008080;"> 24</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 25</span>             <span style="color: #0000ff;">get</span> { <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">new</span> ObjectId(<span style="color: #800000;">"</span><span style="color: #800000;">000000000000000000000000</span><span style="color: #800000;">"</span><span style="color: #000000;">); }
</span><span style="color: #008080;"> 26</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 27</span> 
<span style="color: #008080;"> 28</span>         <span style="color: #0000ff;">internal</span> <span style="color: #0000ff;">byte</span>[] Value { <span style="color: #0000ff;">get</span>; <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">set</span><span style="color: #000000;">; }
</span><span style="color: #008080;"> 29</span> 
<span style="color: #008080;"> 30</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 31</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 获取一个新的</span><span style="color: #808080;">&lt;see cref="ObjectId"/&gt;</span><span style="color: #008000;">。
</span><span style="color: #008080;"> 32</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 33</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;"> 34</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">static</span><span style="color: #000000;"> ObjectId NewId()
</span><span style="color: #008080;"> 35</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 36</span>             <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">new</span> ObjectId { Value =<span style="color: #000000;"> ObjectIdGenerator.Generate() };
</span><span style="color: #008080;"> 37</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 38</span> 
<span style="color: #008080;"> 39</span>         <span style="color: #0000ff;">internal</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">bool</span> TryParse(<span style="color: #0000ff;">string</span> value, <span style="color: #0000ff;">out</span><span style="color: #000000;"> ObjectId objectId)
</span><span style="color: #008080;"> 40</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 41</span>             objectId =<span style="color: #000000;"> Empty;
</span><span style="color: #008080;"> 42</span>             <span style="color: #0000ff;">if</span> (value == <span style="color: #0000ff;">null</span> || value.Length != <span style="color: #800080;">24</span><span style="color: #000000;">)
</span><span style="color: #008080;"> 43</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 44</span>                 <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">false</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 45</span> <span style="color: #000000;">            }
</span><span style="color: #008080;"> 46</span> 
<span style="color: #008080;"> 47</span>             <span style="color: #0000ff;">try</span>
<span style="color: #008080;"> 48</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 49</span>                 objectId = <span style="color: #0000ff;">new</span><span style="color: #000000;"> ObjectId(value);
</span><span style="color: #008080;"> 50</span>                 <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">true</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 51</span> <span style="color: #000000;">            }
</span><span style="color: #008080;"> 52</span>             <span style="color: #0000ff;">catch</span><span style="color: #000000;"> (FormatException)
</span><span style="color: #008080;"> 53</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 54</span>                 <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">false</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 55</span> <span style="color: #000000;">            }
</span><span style="color: #008080;"> 56</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 57</span> 
<span style="color: #008080;"> 58</span>         <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">byte</span>[] DecodeHex(<span style="color: #0000ff;">string</span><span style="color: #000000;"> value)
</span><span style="color: #008080;"> 59</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 60</span>             <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">string</span><span style="color: #000000;">.IsNullOrEmpty(value))
</span><span style="color: #008080;"> 61</span>                 <span style="color: #0000ff;">throw</span> <span style="color: #0000ff;">new</span> ArgumentNullException(<span style="color: #800000;">"</span><span style="color: #800000;">value</span><span style="color: #800000;">"</span><span style="color: #000000;">);
</span><span style="color: #008080;"> 62</span> 
<span style="color: #008080;"> 63</span>             <span style="color: #0000ff;">var</span> chars =<span style="color: #000000;"> value.ToCharArray();
</span><span style="color: #008080;"> 64</span>             <span style="color: #0000ff;">var</span> numberChars =<span style="color: #000000;"> chars.Length;
</span><span style="color: #008080;"> 65</span>             <span style="color: #0000ff;">var</span> bytes = <span style="color: #0000ff;">new</span> <span style="color: #0000ff;">byte</span>[numberChars / <span style="color: #800080;">2</span><span style="color: #000000;">];
</span><span style="color: #008080;"> 66</span> 
<span style="color: #008080;"> 67</span>             <span style="color: #0000ff;">for</span> (<span style="color: #0000ff;">var</span> i = <span style="color: #800080;">0</span>; i &lt; numberChars; i += <span style="color: #800080;">2</span><span style="color: #000000;">)
</span><span style="color: #008080;"> 68</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 69</span>                 bytes[i / <span style="color: #800080;">2</span>] = Convert.ToByte(<span style="color: #0000ff;">new</span> <span style="color: #0000ff;">string</span>(chars, i, <span style="color: #800080;">2</span>), <span style="color: #800080;">16</span><span style="color: #000000;">);
</span><span style="color: #008080;"> 70</span> <span style="color: #000000;">            }
</span><span style="color: #008080;"> 71</span> 
<span style="color: #008080;"> 72</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> bytes;
</span><span style="color: #008080;"> 73</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 74</span> 
<span style="color: #008080;"> 75</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 76</span>         <span style="color: #808080;">///</span> 
<span style="color: #008080;"> 77</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 78</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;"> 79</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">int</span><span style="color: #000000;"> GetHashCode()
</span><span style="color: #008080;"> 80</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 81</span>             <span style="color: #0000ff;">return</span> Value != <span style="color: #0000ff;">null</span> ? ToString().GetHashCode() : <span style="color: #800080;">0</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 82</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 83</span> 
<span style="color: #008080;"> 84</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 85</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 显示此对象的信息，便于调试。
</span><span style="color: #008080;"> 86</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 87</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;"> 88</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">string</span><span style="color: #000000;"> ToString()
</span><span style="color: #008080;"> 89</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 90</span>             <span style="color: #0000ff;">if</span> (_string == <span style="color: #0000ff;">null</span> &amp;&amp; Value != <span style="color: #0000ff;">null</span><span style="color: #000000;">)
</span><span style="color: #008080;"> 91</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 92</span>                 _string =<span style="color: #000000;"> BitConverter.ToString(Value)
</span><span style="color: #008080;"> 93</span>                   .Replace(<span style="color: #800000;">"</span><span style="color: #800000;">-</span><span style="color: #800000;">"</span>, <span style="color: #0000ff;">string</span><span style="color: #000000;">.Empty)
</span><span style="color: #008080;"> 94</span> <span style="color: #000000;">                  .ToLowerInvariant();
</span><span style="color: #008080;"> 95</span> <span style="color: #000000;">            }
</span><span style="color: #008080;"> 96</span> 
<span style="color: #008080;"> 97</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> _string;
</span><span style="color: #008080;"> 98</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 99</span> 
<span style="color: #008080;">100</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">101</span>         <span style="color: #808080;">///</span> 
<span style="color: #008080;">102</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">103</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="obj"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">104</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;">105</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">bool</span> Equals(<span style="color: #0000ff;">object</span><span style="color: #000000;"> obj)
</span><span style="color: #008080;">106</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">107</span>             <span style="color: #0000ff;">var</span> other = obj <span style="color: #0000ff;">as</span><span style="color: #000000;"> ObjectId;
</span><span style="color: #008080;">108</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> Equals(other);
</span><span style="color: #008080;">109</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">110</span> 
<span style="color: #008080;">111</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">112</span>         <span style="color: #808080;">///</span> 
<span style="color: #008080;">113</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">114</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="other"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">115</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;">116</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">bool</span><span style="color: #000000;"> Equals(ObjectId other)
</span><span style="color: #008080;">117</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">118</span>             <span style="color: #0000ff;">return</span> other != <span style="color: #0000ff;">null</span> &amp;&amp; ToString() ==<span style="color: #000000;"> other.ToString();
</span><span style="color: #008080;">119</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">120</span> 
<span style="color: #008080;">121</span>         <span style="color: #008000;">//</span><span style="color: #008000;">public static implicit operator string(ObjectId objectId)
</span><span style="color: #008080;">122</span>         <span style="color: #008000;">//</span><span style="color: #008000;">{
</span><span style="color: #008080;">123</span>         <span style="color: #008000;">//</span><span style="color: #008000;">    return objectId == null ? null : objectId.ToString();
</span><span style="color: #008080;">124</span>         <span style="color: #008000;">//</span><span style="color: #008000;">}
</span><span style="color: #008080;">125</span> 
<span style="color: #008080;">126</span>         <span style="color: #008000;">//</span><span style="color: #008000;">public static implicit operator ObjectId(string value)
</span><span style="color: #008080;">127</span>         <span style="color: #008000;">//</span><span style="color: #008000;">{
</span><span style="color: #008080;">128</span>         <span style="color: #008000;">//</span><span style="color: #008000;">    return new ObjectId(value);
</span><span style="color: #008080;">129</span>         <span style="color: #008000;">//</span><span style="color: #008000;">}</span>
<span style="color: #008080;">130</span> 
<span style="color: #008080;">131</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">132</span>         <span style="color: #808080;">///</span> 
<span style="color: #008080;">133</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">134</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="left"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">135</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="right"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">136</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;">137</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">bool</span> <span style="color: #0000ff;">operator</span> ==<span style="color: #000000;">(ObjectId left, ObjectId right)
</span><span style="color: #008080;">138</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">139</span>             <span style="color: #0000ff;">if</span><span style="color: #000000;"> (ReferenceEquals(left, right))
</span><span style="color: #008080;">140</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">141</span>                 <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">true</span><span style="color: #000000;">;
</span><span style="color: #008080;">142</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">143</span> 
<span style="color: #008080;">144</span>             <span style="color: #0000ff;">if</span> (((<span style="color: #0000ff;">object</span>)left == <span style="color: #0000ff;">null</span>) || ((<span style="color: #0000ff;">object</span>)right == <span style="color: #0000ff;">null</span><span style="color: #000000;">))
</span><span style="color: #008080;">145</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">146</span>                 <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">false</span><span style="color: #000000;">;
</span><span style="color: #008080;">147</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">148</span> 
<span style="color: #008080;">149</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> left.Equals(right);
</span><span style="color: #008080;">150</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">151</span> 
<span style="color: #008080;">152</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">153</span>         <span style="color: #808080;">///</span> 
<span style="color: #008080;">154</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">155</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="left"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">156</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="right"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">157</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;">158</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">bool</span> <span style="color: #0000ff;">operator</span> !=<span style="color: #000000;">(ObjectId left, ObjectId right)
</span><span style="color: #008080;">159</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">160</span>             <span style="color: #0000ff;">return</span> !(left ==<span style="color: #000000;"> right);
</span><span style="color: #008080;">161</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">162</span> 
<span style="color: #008080;">163</span>         <span style="color: #0000ff;">#region</span> ISerializable 成员
<span style="color: #008080;">164</span> 
<span style="color: #008080;">165</span>         <span style="color: #0000ff;">const</span> <span style="color: #0000ff;">string</span> strValue = <span style="color: #800000;">""</span><span style="color: #000000;">;
</span><span style="color: #008080;">166</span>         <span style="color: #0000ff;">void</span><span style="color: #000000;"> ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
</span><span style="color: #008080;">167</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">168</span>             <span style="color: #0000ff;">string</span> value = <span style="color: #0000ff;">this</span><span style="color: #000000;">.ToString();
</span><span style="color: #008080;">169</span> <span style="color: #000000;">            info.AddValue(strValue, value);
</span><span style="color: #008080;">170</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">171</span> 
<span style="color: #008080;">172</span>         <span style="color: #0000ff;">#endregion</span>
<span style="color: #008080;">173</span> 
<span style="color: #008080;">174</span>         <span style="color: #0000ff;">private</span><span style="color: #000000;"> ObjectId(SerializationInfo info, StreamingContext context)
</span><span style="color: #008080;">175</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">176</span>             <span style="color: #0000ff;">string</span> value =<span style="color: #000000;"> info.GetString(strValue);
</span><span style="color: #008080;">177</span>             <span style="color: #0000ff;">this</span>.Value =<span style="color: #000000;"> DecodeHex(value);
</span><span style="color: #008080;">178</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">179</span> 
<span style="color: #008080;">180</span> 
<span style="color: #008080;">181</span>         <span style="color: #0000ff;">#region</span> IComparable&lt;ObjectId&gt; 成员
<span style="color: #008080;">182</span> 
<span style="color: #008080;">183</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">184</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 根据</span><span style="color: #808080;">&lt;see cref="ObjectId.ToString()"/&gt;</span><span style="color: #008000;">的值比较两个对象。
</span><span style="color: #008080;">185</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">186</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="other"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">187</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;">188</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">int</span><span style="color: #000000;"> CompareTo(ObjectId other)
</span><span style="color: #008080;">189</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">190</span>             <span style="color: #0000ff;">if</span> (other == <span style="color: #0000ff;">null</span>) { <span style="color: #0000ff;">return</span> <span style="color: #800080;">1</span><span style="color: #000000;">; }
</span><span style="color: #008080;">191</span> 
<span style="color: #008080;">192</span>             <span style="color: #0000ff;">string</span> thisStr = <span style="color: #0000ff;">this</span><span style="color: #000000;">.ToString();
</span><span style="color: #008080;">193</span>             <span style="color: #0000ff;">string</span> otherStr =<span style="color: #000000;"> other.ToString();
</span><span style="color: #008080;">194</span>             <span style="color: #0000ff;">int</span> result =<span style="color: #000000;"> thisStr.CompareTo(otherStr);
</span><span style="color: #008080;">195</span> 
<span style="color: #008080;">196</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> result;
</span><span style="color: #008080;">197</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">198</span> 
<span style="color: #008080;">199</span>         <span style="color: #0000ff;">#endregion</span>
<span style="color: #008080;">200</span> 
<span style="color: #008080;">201</span>         <span style="color: #0000ff;">#region</span> IComparable 成员
<span style="color: #008080;">202</span> 
<span style="color: #008080;">203</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">204</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 根据</span><span style="color: #808080;">&lt;see cref="ObjectId.ToString()"/&gt;</span><span style="color: #008000;">的值比较两个对象。
</span><span style="color: #008080;">205</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">206</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="obj"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">207</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;">208</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">int</span> CompareTo(<span style="color: #0000ff;">object</span><span style="color: #000000;"> obj)
</span><span style="color: #008080;">209</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">210</span>             ObjectId other = obj <span style="color: #0000ff;">as</span><span style="color: #000000;"> ObjectId;
</span><span style="color: #008080;">211</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> CompareTo(other);
</span><span style="color: #008080;">212</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">213</span> 
<span style="color: #008080;">214</span>         <span style="color: #0000ff;">#endregion</span>
<span style="color: #008080;">215</span> <span style="color: #000000;">    }
</span><span style="color: #008080;">216</span> 
<span style="color: #008080;">217</span>     <span style="color: #0000ff;">internal</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">class</span><span style="color: #000000;"> ObjectIdGenerator
</span><span style="color: #008080;">218</span> <span style="color: #000000;">    {
</span><span style="color: #008080;">219</span>         <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">readonly</span> DateTime Epoch =
<span style="color: #008080;">220</span>           <span style="color: #0000ff;">new</span> DateTime(<span style="color: #800080;">1970</span>, <span style="color: #800080;">1</span>, <span style="color: #800080;">1</span>, <span style="color: #800080;">0</span>, <span style="color: #800080;">0</span>, <span style="color: #800080;">0</span><span style="color: #000000;">, DateTimeKind.Utc);
</span><span style="color: #008080;">221</span>         <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">readonly</span> <span style="color: #0000ff;">object</span> _innerLock = <span style="color: #0000ff;">new</span> <span style="color: #0000ff;">object</span><span style="color: #000000;">();
</span><span style="color: #008080;">222</span>         <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">int</span><span style="color: #000000;"> _counter;
</span><span style="color: #008080;">223</span>         <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">readonly</span> <span style="color: #0000ff;">byte</span>[] _machineHash =<span style="color: #000000;"> GenerateHostHash();
</span><span style="color: #008080;">224</span>         <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">readonly</span> <span style="color: #0000ff;">byte</span>[] _processId =
<span style="color: #008080;">225</span> <span style="color: #000000;">          BitConverter.GetBytes(GenerateProcessId());
</span><span style="color: #008080;">226</span> 
<span style="color: #008080;">227</span>         <span style="color: #0000ff;">internal</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">byte</span><span style="color: #000000;">[] Generate()
</span><span style="color: #008080;">228</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">229</span>             <span style="color: #0000ff;">var</span> oid = <span style="color: #0000ff;">new</span> <span style="color: #0000ff;">byte</span>[<span style="color: #800080;">12</span><span style="color: #000000;">];
</span><span style="color: #008080;">230</span>             <span style="color: #0000ff;">var</span> copyidx = <span style="color: #800080;">0</span><span style="color: #000000;">;
</span><span style="color: #008080;">231</span> 
<span style="color: #008080;">232</span>             Array.Copy(BitConverter.GetBytes(GenerateTime()), <span style="color: #800080;">0</span>, oid, copyidx, <span style="color: #800080;">4</span><span style="color: #000000;">);
</span><span style="color: #008080;">233</span>             copyidx += <span style="color: #800080;">4</span><span style="color: #000000;">;
</span><span style="color: #008080;">234</span> 
<span style="color: #008080;">235</span>             Array.Copy(_machineHash, <span style="color: #800080;">0</span>, oid, copyidx, <span style="color: #800080;">3</span><span style="color: #000000;">);
</span><span style="color: #008080;">236</span>             copyidx += <span style="color: #800080;">3</span><span style="color: #000000;">;
</span><span style="color: #008080;">237</span> 
<span style="color: #008080;">238</span>             Array.Copy(_processId, <span style="color: #800080;">0</span>, oid, copyidx, <span style="color: #800080;">2</span><span style="color: #000000;">);
</span><span style="color: #008080;">239</span>             copyidx += <span style="color: #800080;">2</span><span style="color: #000000;">;
</span><span style="color: #008080;">240</span> 
<span style="color: #008080;">241</span>             Array.Copy(BitConverter.GetBytes(GenerateCounter()), <span style="color: #800080;">0</span>, oid, copyidx, <span style="color: #800080;">3</span><span style="color: #000000;">);
</span><span style="color: #008080;">242</span> 
<span style="color: #008080;">243</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> oid;
</span><span style="color: #008080;">244</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">245</span> 
<span style="color: #008080;">246</span>         <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">int</span><span style="color: #000000;"> GenerateTime()
</span><span style="color: #008080;">247</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">248</span>             <span style="color: #0000ff;">var</span> now =<span style="color: #000000;"> DateTime.UtcNow;
</span><span style="color: #008080;">249</span>             <span style="color: #0000ff;">var</span> nowtime = <span style="color: #0000ff;">new</span><span style="color: #000000;"> DateTime(Epoch.Year, Epoch.Month, Epoch.Day,
</span><span style="color: #008080;">250</span> <span style="color: #000000;">              now.Hour, now.Minute, now.Second, now.Millisecond);
</span><span style="color: #008080;">251</span>             <span style="color: #0000ff;">var</span> diff = nowtime -<span style="color: #000000;"> Epoch;
</span><span style="color: #008080;">252</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> Convert.ToInt32(Math.Floor(diff.TotalMilliseconds));
</span><span style="color: #008080;">253</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">254</span> 
<span style="color: #008080;">255</span>         <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">byte</span><span style="color: #000000;">[] GenerateHostHash()
</span><span style="color: #008080;">256</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">257</span>             <span style="color: #0000ff;">using</span> (<span style="color: #0000ff;">var</span> md5 =<span style="color: #000000;"> MD5.Create())
</span><span style="color: #008080;">258</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">259</span>                 <span style="color: #0000ff;">var</span> host =<span style="color: #000000;"> Dns.GetHostName();
</span><span style="color: #008080;">260</span>                 <span style="color: #0000ff;">return</span><span style="color: #000000;"> md5.ComputeHash(Encoding.Default.GetBytes(host));
</span><span style="color: #008080;">261</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">262</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">263</span> 
<span style="color: #008080;">264</span>         <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">int</span><span style="color: #000000;"> GenerateProcessId()
</span><span style="color: #008080;">265</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">266</span>             <span style="color: #0000ff;">var</span> process =<span style="color: #000000;"> Process.GetCurrentProcess();
</span><span style="color: #008080;">267</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> process.Id;
</span><span style="color: #008080;">268</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">269</span> 
<span style="color: #008080;">270</span>         <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">int</span><span style="color: #000000;"> GenerateCounter()
</span><span style="color: #008080;">271</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">272</span>             <span style="color: #0000ff;">lock</span><span style="color: #000000;"> (_innerLock)
</span><span style="color: #008080;">273</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">274</span>                 <span style="color: #0000ff;">return</span> _counter++<span style="color: #000000;">;
</span><span style="color: #008080;">275</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">276</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">277</span>     }</pre>
</div>
<span class="cnblogs_code_collapse">ObjectId</span></div>
<p>&nbsp;</p>
<p><span style="line-height: 1.5;">使用时只需通过调用</span><span style="color: #2b91af;"><span style="font-family: Lucida Console; font-size: 9pt; background-color: white;">ObjectId<span style="color: black;">.NewId<span style="color: #ff9900;">()<span style="color: black;">;</span></span></span></span></span>即可获取一个新的编号。</p>
<h2>分页机制</h2>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810227833027.png" alt="" /></p>
<p>磁盘I/O操作每次都是以4KB个字节为单位进行的。所以把单文件数据库划分为一个个长度为4KB的页就很有必要。这一点稍微增加了数据库设计图的复杂程度。由于磁盘I/O所需时间最长，所以对此进行优化是值得的。</p>
<p>你可以随意新建一个TXT文件，在里面写几个字符，保存一下，会看到即使是大小只有1个字节内容的TXT文件，其占用空间也是4KB。</p>
<p style="background: white;"><img src="http://images0.cnblogs.com/blog/383191/201507/111810259088239.png" alt="" /></p>
<p>而且所有文件的"占用空间"都是4KB的整数倍。</p>
<p>&nbsp;</p>
<h2>索引机制</h2>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810272832253.png" alt="" /></p>
<p>我从LiteDB的文档看到，它用Skip List实现了索引机制，能够快速定位读写一个对象。Skip List是以空间换时间的方式，用扩展了的单链表达到了红黑树的效率，而其代码比红黑树简单得多。要研究、实现红黑树会花费更多时间，所以我效仿LiteDB用Skip List做索引。</p>
<p>关于Skip List大家可以参考<a href="http://www.codeproject.com/Articles/16337/Back-to-Basics-Generic-Data-Structures-and-Algorit">这里</a>，有Skip List的实现代码。（还有很多数据结构和算法的C#实现，堪称宝贵）还有这里（<a href="http://www.cnblogs.com/xuqiang/archive/2011/05/22/2053516.html">http://www.cnblogs.com/xuqiang/archive/2011/05/22/2053516.html</a>）的介绍也很不错。</p>
<p>Skip List的结构如下图所示。</p>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810280647081.png" alt="" /></p>
<p><img src="http://images.cnblogs.com/cnblogs_com/bitzhuwei/482613/o_Skip_list_add_element-en.gif" alt="" width="500" height="173" /></p>
<p>你只需知道Skip List在外部看起来就像一个Dictionary&lt;TKey, TValue&gt;，它是通过Add(TKey key, TValue value);来增加元素的。每个Skip List Node都含有一个key和一个value，而且，同一列上的结点的key和value值都相同。例如，上图的key值为50的三个Skip List Node，其key当然都是50，而其value也必须是相同的。</p>
<p>关于Skip List的详细介绍可参考<a href="https://en.wikipedia.org/wiki/Skip_list">维基百科</a>。</p>
<p>&nbsp;</p>
<h2>查询语句</h2>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810297218308.png" alt="" /></p>
<p>创建数据库、创建表、索引和删除表的语句都已经不需要了。</p>
<p>Lambda表达式可以用作查询语句。再次感谢LiteDB，给了我很多启发。</p>
<h3>不利用索引的懒惰方案</h3>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810302211223.png" alt="" /></p>
<p>解析Lambda表达式的工作量超出我的预期，暂时先用一个懒惰的方案顶替之。LiteDB提供的解析方式也有很大局限，我还要考虑一下如何做Lambda表达式的解析。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 2</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 查找数据库内的某些记录。
</span><span style="color: #008080;"> 3</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 4</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;typeparam name="T"&gt;&lt;/typeparam&gt;</span>
<span style="color: #008080;"> 5</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="predicate"&gt;</span><span style="color: #008000;">符合此条件的记录会被取出。</span><span style="color: #808080;">&lt;/param&gt;</span>
<span style="color: #008080;"> 6</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;"> 7</span>         <span style="color: #0000ff;">public</span> IEnumerable&lt;T&gt; Find&lt;T&gt;(Expression&lt;Func&lt;T, <span style="color: #0000ff;">bool</span>&gt;&gt; predicate) <span style="color: #0000ff;">where</span> T : Table, <span style="color: #0000ff;">new</span><span style="color: #000000;">()
</span><span style="color: #008080;"> 8</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 9</span>             <span style="color: #0000ff;">if</span> (predicate == <span style="color: #0000ff;">null</span>) { <span style="color: #0000ff;">throw</span> <span style="color: #0000ff;">new</span> ArgumentNullException(<span style="color: #800000;">"</span><span style="color: #800000;">predicate</span><span style="color: #800000;">"</span><span style="color: #000000;">); }
</span><span style="color: #008080;">10</span> 
<span style="color: #008080;">11</span>             <span style="color: #008000;">//</span><span style="color: #008000;"> 这是没有利用索引的版本。</span>
<span style="color: #008080;">12</span>             Func&lt;T, <span style="color: #0000ff;">bool</span>&gt; func =<span style="color: #000000;"> predicate.Compile();
</span><span style="color: #008080;">13</span>             <span style="color: #0000ff;">foreach</span> (T item <span style="color: #0000ff;">in</span> <span style="color: #0000ff;">this</span>.FindAll&lt;T&gt;<span style="color: #000000;">())
</span><span style="color: #008080;">14</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">15</span>                 <span style="color: #0000ff;">if</span><span style="color: #000000;">(func(item))
</span><span style="color: #008080;">16</span> <span style="color: #000000;">                {
</span><span style="color: #008080;">17</span>                     <span style="color: #0000ff;">yield</span> <span style="color: #0000ff;">return</span><span style="color: #000000;"> item;
</span><span style="color: #008080;">18</span> <span style="color: #000000;">                }
</span><span style="color: #008080;">19</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">20</span> 
<span style="color: #008080;">21</span>             <span style="color: #008000;">//</span><span style="color: #008000;"> TODO: 这是利用索引的版本，尚未实现。
</span><span style="color: #008080;">22</span>             <span style="color: #008000;">//</span><span style="color: #008000;">List&lt;T&gt; result = new List&lt;T&gt;();
</span><span style="color: #008080;">23</span> 
<span style="color: #008080;">24</span>             <span style="color: #008000;">//</span><span style="color: #008000;">var body = predicate.Body as LambdaExpression;
</span><span style="color: #008080;">25</span>             <span style="color: #008000;">//</span><span style="color: #008000;">this.Find(result, body);
</span><span style="color: #008080;">26</span> 
<span style="color: #008080;">27</span>             <span style="color: #008000;">//</span><span style="color: #008000;">return result;</span>
<span style="color: #008080;">28</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">29</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">30</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 查找数据库内所有指定类型的记录。
</span><span style="color: #008080;">31</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">32</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;typeparam name="T"&gt;</span><span style="color: #008000;">要查找的类型。</span><span style="color: #808080;">&lt;/typeparam&gt;</span>
<span style="color: #008080;">33</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;">34</span>         <span style="color: #0000ff;">public</span> IEnumerable&lt;T&gt; FindAll&lt;T&gt;() <span style="color: #0000ff;">where</span> T:Table, <span style="color: #0000ff;">new</span><span style="color: #000000;">()
</span><span style="color: #008080;">35</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">36</span>             Type type = <span style="color: #0000ff;">typeof</span><span style="color: #000000;">(T);
</span><span style="color: #008080;">37</span>             <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span><span style="color: #000000;">.tableBlockDict.ContainsKey(type))
</span><span style="color: #008080;">38</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">39</span>                 TableBlock tableBlock = <span style="color: #0000ff;">this</span><span style="color: #000000;">.tableBlockDict[type];
</span><span style="color: #008080;">40</span>                 IndexBlock firstIndex = tableBlock.IndexBlockHead.NextObj;<span style="color: #008000;">//</span><span style="color: #008000;"> 第一个索引应该是Table.Id的索引。</span>
<span style="color: #008080;">41</span>                 FileStream fs = <span style="color: #0000ff;">this</span><span style="color: #000000;">.fileStream;
</span><span style="color: #008080;">42</span> 
<span style="color: #008080;">43</span>                 SkipListNodeBlock current = firstIndex.SkipListHeadNodes[<span style="color: #800080;">0</span>]; <span style="color: #008000;">//</span><span style="color: #008000;">currentHeadNode;</span>
<span style="color: #008080;">44</span> 
<span style="color: #008080;">45</span>                 <span style="color: #0000ff;">while</span> (current.RightPos !=<span style="color: #000000;"> firstIndex.SkipListTailNodePos)
</span><span style="color: #008080;">46</span> <span style="color: #000000;">                {
</span><span style="color: #008080;">47</span> <span style="color: #000000;">                    current.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.RightObj);
</span><span style="color: #008080;">48</span>                     current.RightObj.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.RightObj |<span style="color: #000000;"> SkipListNodeBlockLoadOptions.Value);
</span><span style="color: #008080;">49</span>                     T item = current.RightObj.Value.GetObject&lt;T&gt;(<span style="color: #0000ff;">this</span><span style="color: #000000;">);
</span><span style="color: #008080;">50</span> 
<span style="color: #008080;">51</span>                     <span style="color: #0000ff;">yield</span> <span style="color: #0000ff;">return</span><span style="color: #000000;"> item;
</span><span style="color: #008080;">52</span> 
<span style="color: #008080;">53</span>                     current =<span style="color: #000000;"> current.RightObj;
</span><span style="color: #008080;">54</span> <span style="color: #000000;">                }
</span><span style="color: #008080;">55</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">56</span>         }</pre>
</div>
<p>&nbsp;</p>
<h3>Lambda表达式</h3>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810315023264.png" alt="" /></p>
<p>在MSDN上有观察Lambda表达式的介绍。</p>
<p><a title="单击以折叠。双击以全部折叠。" href="javascript:void(0)">继承层次结构</a></p>
<p><a href="https://msdn.microsoft.com/zh-cn/library/system.object.aspx">System.Object</a><br />&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.expression.aspx">System.Linq.Expressions.Expression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.binaryexpression.aspx">System.Linq.Expressions.BinaryExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.blockexpression.aspx">System.Linq.Expressions.BlockExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.conditionalexpression.aspx">System.Linq.Expressions.ConditionalExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.constantexpression.aspx">System.Linq.Expressions.ConstantExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.debuginfoexpression.aspx">System.Linq.Expressions.DebugInfoExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.defaultexpression.aspx">System.Linq.Expressions.DefaultExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.dynamicexpression.aspx">System.Linq.Expressions.DynamicExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.gotoexpression.aspx">System.Linq.Expressions.GotoExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.indexexpression.aspx">System.Linq.Expressions.IndexExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.invocationexpression.aspx">System.Linq.Expressions.InvocationExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.labelexpression.aspx">System.Linq.Expressions.LabelExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.lambdaexpression.aspx">System.Linq.Expressions.LambdaExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.listinitexpression.aspx">System.Linq.Expressions.ListInitExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.loopexpression.aspx">System.Linq.Expressions.LoopExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.memberexpression.aspx">System.Linq.Expressions.MemberExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.memberinitexpression.aspx">System.Linq.Expressions.MemberInitExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.methodcallexpression.aspx">System.Linq.Expressions.MethodCallExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.newarrayexpression.aspx">System.Linq.Expressions.NewArrayExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.newexpression.aspx">System.Linq.Expressions.NewExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.parameterexpression.aspx">System.Linq.Expressions.ParameterExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.runtimevariablesexpression.aspx">System.Linq.Expressions.RuntimeVariablesExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.switchexpression.aspx">System.Linq.Expressions.SwitchExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.tryexpression.aspx">System.Linq.Expressions.TryExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.typebinaryexpression.aspx">System.Linq.Expressions.TypeBinaryExpression</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://msdn.microsoft.com/zh-cn/library/system.linq.expressions.unaryexpression.aspx">System.Linq.Expressions.UnaryExpression</a>
	</p>
<p>这个列表放在这里是为了方便了解lambda表达式都有哪些类型的结点。我还整理了描述表达式目录树的节点的节点类型System.Linq.Expressions. ExpressionType。
</p>
<div class="cnblogs_code" onclick="cnblogs_code_show('53b8af07-b799-48bb-b1db-0ba727004b22')"><img id="code_img_closed_53b8af07-b799-48bb-b1db-0ba727004b22" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_53b8af07-b799-48bb-b1db-0ba727004b22" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('53b8af07-b799-48bb-b1db-0ba727004b22',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_53b8af07-b799-48bb-b1db-0ba727004b22" class="cnblogs_code_hide">
<pre><span style="color: #008080;">  1</span> <span style="color: #0000ff;">namespace</span><span style="color: #000000;"> System.Linq.Expressions
</span><span style="color: #008080;">  2</span> <span style="color: #000000;">{
</span><span style="color: #008080;">  3</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">  4</span>     <span style="color: #808080;">///</span><span style="color: #008000;"> 描述表达式目录树的节点的节点类型。
</span><span style="color: #008080;">  5</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">  6</span>     <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">enum</span><span style="color: #000000;"> ExpressionType
</span><span style="color: #008080;">  7</span> <span style="color: #000000;">    {
</span><span style="color: #008080;">  8</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">  9</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 加法运算，如 a + b，针对数值操作数，不进行溢出检查。
</span><span style="color: #008080;"> 10</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 11</span>         Add = <span style="color: #800080;">0</span><span style="color: #000000;">,
</span><span style="color: #008080;"> 12</span>         <span style="color: #008000;">//
</span><span style="color: #008080;"> 13</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 14</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 加法运算，如 (a + b)，针对数值操作数，进行溢出检查。
</span><span style="color: #008080;"> 15</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 16</span>         AddChecked = <span style="color: #800080;">1</span><span style="color: #000000;">,
</span><span style="color: #008080;"> 17</span>         <span style="color: #008000;">//
</span><span style="color: #008080;"> 18</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 19</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 按位或逻辑 AND 运算，如 C# 中的 (a &amp; b) 和 Visual Basic 中的 (a And b)。
</span><span style="color: #008080;"> 20</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 21</span>         And = <span style="color: #800080;">2</span><span style="color: #000000;">,
</span><span style="color: #008080;"> 22</span>         <span style="color: #008000;">//
</span><span style="color: #008080;"> 23</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 24</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 条件 AND 运算，它仅在第一个操作数的计算结果为 true 时才计算第二个操作数。 它与 C# 中的 (a &amp;&amp; b) 和 Visual Basic 中的 (a AndAlso b) 对应。
</span><span style="color: #008080;"> 25</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 26</span>         AndAlso = <span style="color: #800080;">3</span><span style="color: #000000;">,
</span><span style="color: #008080;"> 27</span>         <span style="color: #008000;">//
</span><span style="color: #008080;"> 28</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 29</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 获取一维数组长度的运算，如 array.Length。
</span><span style="color: #008080;"> 30</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 31</span>         ArrayLength = <span style="color: #800080;">4</span><span style="color: #000000;">,
</span><span style="color: #008080;"> 32</span>         <span style="color: #008000;">//
</span><span style="color: #008080;"> 33</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 34</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 一维数组中的索引运算，如 C# 中的 array[index] 或 Visual Basic 中的 array(index)。
</span><span style="color: #008080;"> 35</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 36</span>         ArrayIndex = <span style="color: #800080;">5</span><span style="color: #000000;">,
</span><span style="color: #008080;"> 37</span>         <span style="color: #008000;">//
</span><span style="color: #008080;"> 38</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 39</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 方法调用，如在 obj.sampleMethod() 表达式中。
</span><span style="color: #008080;"> 40</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 41</span>         Call = <span style="color: #800080;">6</span><span style="color: #000000;">,
</span><span style="color: #008080;"> 42</span>         <span style="color: #008000;">//
</span><span style="color: #008080;"> 43</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 44</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 表示 null 合并运算的节点，如 C# 中的 (a ?? b) 或 Visual Basic 中的 If(a, b)。
</span><span style="color: #008080;"> 45</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 46</span>         Coalesce = <span style="color: #800080;">7</span><span style="color: #000000;">,
</span><span style="color: #008080;"> 47</span>         <span style="color: #008000;">//
</span><span style="color: #008080;"> 48</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 49</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 条件运算，如 C# 中的 a &gt; b ? a : b 或 Visual Basic 中的 If(a &gt; b, a, b)。
</span><span style="color: #008080;"> 50</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 51</span>         Conditional = <span style="color: #800080;">8</span><span style="color: #000000;">,
</span><span style="color: #008080;"> 52</span>         <span style="color: #008000;">//
</span><span style="color: #008080;"> 53</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 54</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 一个常量值。
</span><span style="color: #008080;"> 55</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 56</span>         Constant = <span style="color: #800080;">9</span><span style="color: #000000;">,
</span><span style="color: #008080;"> 57</span>         <span style="color: #008000;">//
</span><span style="color: #008080;"> 58</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 59</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 强制转换或转换运算，如 C#中的 (SampleType)obj 或 Visual Basic 中的 CType(obj, SampleType)。
</span><span style="color: #008080;"> 60</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 对于数值转换，如果转换后的值对于目标类型来说太大，这不会引发异常。
</span><span style="color: #008080;"> 61</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 62</span>         Convert = <span style="color: #800080;">10</span><span style="color: #000000;">,
</span><span style="color: #008080;"> 63</span>         <span style="color: #008000;">//
</span><span style="color: #008080;"> 64</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 65</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 强制转换或转换运算，如 C#中的 (SampleType)obj 或 Visual Basic 中的 CType(obj, SampleType)。
</span><span style="color: #008080;"> 66</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 对于数值转换，如果转换后的值与目标类型大小不符，则引发异常。
</span><span style="color: #008080;"> 67</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 68</span>         ConvertChecked = <span style="color: #800080;">11</span><span style="color: #000000;">,
</span><span style="color: #008080;"> 69</span>         <span style="color: #008000;">//
</span><span style="color: #008080;"> 70</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 71</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 除法运算，如 (a / b)，针对数值操作数。
</span><span style="color: #008080;"> 72</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 73</span>         Divide = <span style="color: #800080;">12</span><span style="color: #000000;">,
</span><span style="color: #008080;"> 74</span>         <span style="color: #008000;">//
</span><span style="color: #008080;"> 75</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 76</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 表示相等比较的节点，如 C# 中的 (a == b) 或 Visual Basic 中的 (a = b)。
</span><span style="color: #008080;"> 77</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 78</span>         Equal = <span style="color: #800080;">13</span><span style="color: #000000;">,
</span><span style="color: #008080;"> 79</span>         <span style="color: #008000;">//
</span><span style="color: #008080;"> 80</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 81</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 按位或逻辑 XOR 运算，如 C# 中的 (a ^ b) 或 Visual Basic 中的 (a Xor b)。
</span><span style="color: #008080;"> 82</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 83</span>         ExclusiveOr = <span style="color: #800080;">14</span><span style="color: #000000;">,
</span><span style="color: #008080;"> 84</span>         <span style="color: #008000;">//
</span><span style="color: #008080;"> 85</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 86</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> &ldquo;大于&rdquo;比较，如 (a &gt; b)。
</span><span style="color: #008080;"> 87</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 88</span>         GreaterThan = <span style="color: #800080;">15</span><span style="color: #000000;">,
</span><span style="color: #008080;"> 89</span>         <span style="color: #008000;">//
</span><span style="color: #008080;"> 90</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 91</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> &ldquo;大于或等于&rdquo;比较，如 (a &gt;= b)。
</span><span style="color: #008080;"> 92</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 93</span>         GreaterThanOrEqual = <span style="color: #800080;">16</span><span style="color: #000000;">,
</span><span style="color: #008080;"> 94</span>         <span style="color: #008000;">//
</span><span style="color: #008080;"> 95</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 96</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 调用委托或 lambda 表达式的运算，如 sampleDelegate.Invoke()。
</span><span style="color: #008080;"> 97</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 98</span>         Invoke = <span style="color: #800080;">17</span><span style="color: #000000;">,
</span><span style="color: #008080;"> 99</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">100</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">101</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> lambda 表达式，如 C# 中的 a =&gt; a + a 或 Visual Basic 中的 Function(a) a + a。
</span><span style="color: #008080;">102</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">103</span>         Lambda = <span style="color: #800080;">18</span><span style="color: #000000;">,
</span><span style="color: #008080;">104</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">105</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">106</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 按位左移运算，如 (a </span><span style="color: #808080;">&lt;&lt; b)。
</span><span style="color: #008080;">107</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">108</span>         LeftShift = <span style="color: #800080;">19</span><span style="color: #000000;">,
</span><span style="color: #008080;">109</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">110</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">111</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> &ldquo;小于&rdquo;比较，如 (a </span><span style="color: #808080;">&lt; b)。
</span><span style="color: #008080;">112</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">113</span>         LessThan = <span style="color: #800080;">20</span><span style="color: #000000;">,
</span><span style="color: #008080;">114</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">115</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">116</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> &ldquo;小于或等于&rdquo;比较，如 (a </span><span style="color: #808080;">&lt;= b)。
</span><span style="color: #008080;">117</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">118</span>         LessThanOrEqual = <span style="color: #800080;">21</span><span style="color: #000000;">,
</span><span style="color: #008080;">119</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">120</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">121</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 创建新的 System.Collections.IEnumerable 对象并从元素列表中初始化该对象的运算，如 C# 中的 new List</span><span style="color: #808080;">&lt;SampleType&gt;</span><span style="color: #008000;">(){
</span><span style="color: #008080;">122</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> a, b, c } 或 Visual Basic 中的 Dim sampleList = { a, b, c }。
</span><span style="color: #008080;">123</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">124</span>         ListInit = <span style="color: #800080;">22</span><span style="color: #000000;">,
</span><span style="color: #008080;">125</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">126</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">127</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 从字段或属性进行读取的运算，如 obj.SampleProperty。
</span><span style="color: #008080;">128</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">129</span>         MemberAccess = <span style="color: #800080;">23</span><span style="color: #000000;">,
</span><span style="color: #008080;">130</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">131</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">132</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 创建新的对象并初始化其一个或多个成员的运算，如 C# 中的 new Point { X = 1, Y = 2 } 或 Visual Basic 中的
</span><span style="color: #008080;">133</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> New Point With {.X = 1, .Y = 2}。
</span><span style="color: #008080;">134</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">135</span>         MemberInit = <span style="color: #800080;">24</span><span style="color: #000000;">,
</span><span style="color: #008080;">136</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">137</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">138</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 算术余数运算，如 C# 中的 (a % b) 或 Visual Basic 中的 (a Mod b)。
</span><span style="color: #008080;">139</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">140</span>         Modulo = <span style="color: #800080;">25</span><span style="color: #000000;">,
</span><span style="color: #008080;">141</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">142</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">143</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 乘法运算，如 (a * b)，针对数值操作数，不进行溢出检查。
</span><span style="color: #008080;">144</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">145</span>         Multiply = <span style="color: #800080;">26</span><span style="color: #000000;">,
</span><span style="color: #008080;">146</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">147</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">148</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 乘法运算，如 (a * b)，针对数值操作数，进行溢出检查。
</span><span style="color: #008080;">149</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">150</span>         MultiplyChecked = <span style="color: #800080;">27</span><span style="color: #000000;">,
</span><span style="color: #008080;">151</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">152</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">153</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 算术求反运算，如 (-a)。 不应就地修改 a 对象。
</span><span style="color: #008080;">154</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">155</span>         Negate = <span style="color: #800080;">28</span><span style="color: #000000;">,
</span><span style="color: #008080;">156</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">157</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">158</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 一元加法运算，如 (+a)。 预定义的一元加法运算的结果是操作数的值，但用户定义的实现可以产生特殊结果。
</span><span style="color: #008080;">159</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">160</span>         UnaryPlus = <span style="color: #800080;">29</span><span style="color: #000000;">,
</span><span style="color: #008080;">161</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">162</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">163</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 算术求反运算，如 (-a)，进行溢出检查。 不应就地修改 a 对象。
</span><span style="color: #008080;">164</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">165</span>         NegateChecked = <span style="color: #800080;">30</span><span style="color: #000000;">,
</span><span style="color: #008080;">166</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">167</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">168</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 调用构造函数创建新对象的运算，如 new SampleType()。
</span><span style="color: #008080;">169</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">170</span>         New = <span style="color: #800080;">31</span><span style="color: #000000;">,
</span><span style="color: #008080;">171</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">172</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">173</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 创建新的一维数组并从元素列表中初始化该数组的运算，如 C# 中的 new SampleType[]{a, b, c} 或 Visual Basic
</span><span style="color: #008080;">174</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 中的 New SampleType(){a, b, c}。
</span><span style="color: #008080;">175</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">176</span>         NewArrayInit = <span style="color: #800080;">32</span><span style="color: #000000;">,
</span><span style="color: #008080;">177</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">178</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">179</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 创建新数组（其中每个维度的界限均已指定）的运算，如 C# 中的 new SampleType[dim1, dim2] 或 Visual Basic
</span><span style="color: #008080;">180</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 中的 New SampleType(dim1, dim2)。
</span><span style="color: #008080;">181</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">182</span>         NewArrayBounds = <span style="color: #800080;">33</span><span style="color: #000000;">,
</span><span style="color: #008080;">183</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">184</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">185</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 按位求补运算或逻辑求反运算。 在 C# 中，它与整型的 (~a) 和布尔值的 (!a) 等效。 在 Visual Basic 中，它与 (Not
</span><span style="color: #008080;">186</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> a) 等效。 不应就地修改 a 对象。
</span><span style="color: #008080;">187</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">188</span>         Not = <span style="color: #800080;">34</span><span style="color: #000000;">,
</span><span style="color: #008080;">189</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">190</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">191</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 不相等比较，如 C# 中的 (a != b) 或 Visual Basic 中的 (a </span><span style="color: #808080;">&lt;&gt;</span><span style="color: #008000;"> b)。
</span><span style="color: #008080;">192</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">193</span>         NotEqual = <span style="color: #800080;">35</span><span style="color: #000000;">,
</span><span style="color: #008080;">194</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">195</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">196</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 按位或逻辑 OR 运算，如 C# 中的 (a | b) 或 Visual Basic 中的 (a Or b)。
</span><span style="color: #008080;">197</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">198</span>         Or = <span style="color: #800080;">36</span><span style="color: #000000;">,
</span><span style="color: #008080;">199</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">200</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">201</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 短路条件 OR 运算，如 C# 中的 (a || b) 或 Visual Basic 中的 (a OrElse b)。
</span><span style="color: #008080;">202</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">203</span>         OrElse = <span style="color: #800080;">37</span><span style="color: #000000;">,
</span><span style="color: #008080;">204</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">205</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">206</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 对在表达式上下文中定义的参数或变量的引用。 有关更多信息，请参见 System.Linq.Expressions.ParameterExpression。
</span><span style="color: #008080;">207</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">208</span>         Parameter = <span style="color: #800080;">38</span><span style="color: #000000;">,
</span><span style="color: #008080;">209</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">210</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">211</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 对某个数字进行幂运算的数学运算，如 Visual Basic 中的 (a ^ b)。
</span><span style="color: #008080;">212</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">213</span>         Power = <span style="color: #800080;">39</span><span style="color: #000000;">,
</span><span style="color: #008080;">214</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">215</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">216</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 具有类型为 System.Linq.Expressions.Expression 的常量值的表达式。 System.Linq.Expressions.ExpressionType.Quote
</span><span style="color: #008080;">217</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 节点可包含对参数的引用，这些参数在该节点表示的表达式的上下文中定义。
</span><span style="color: #008080;">218</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">219</span>         Quote = <span style="color: #800080;">40</span><span style="color: #000000;">,
</span><span style="color: #008080;">220</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">221</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">222</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 按位右移运算，如 (a &gt;&gt; b)。
</span><span style="color: #008080;">223</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">224</span>         RightShift = <span style="color: #800080;">41</span><span style="color: #000000;">,
</span><span style="color: #008080;">225</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">226</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">227</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 减法运算，如 (a - b)，针对数值操作数，不进行溢出检查。
</span><span style="color: #008080;">228</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">229</span>         Subtract = <span style="color: #800080;">42</span><span style="color: #000000;">,
</span><span style="color: #008080;">230</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">231</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">232</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 算术减法运算，如 (a - b)，针对数值操作数，进行溢出检查。
</span><span style="color: #008080;">233</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">234</span>         SubtractChecked = <span style="color: #800080;">43</span><span style="color: #000000;">,
</span><span style="color: #008080;">235</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">236</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">237</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 显式引用或装箱转换，其中如果转换失败则提供 null，如 C# 中的 (obj as SampleType) 或 Visual Basic 中的
</span><span style="color: #008080;">238</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> TryCast(obj, SampleType)。
</span><span style="color: #008080;">239</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">240</span>         TypeAs = <span style="color: #800080;">44</span><span style="color: #000000;">,
</span><span style="color: #008080;">241</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">242</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">243</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 类型测试，如 C# 中的 obj is SampleType 或 Visual Basic 中的 TypeOf obj is SampleType。
</span><span style="color: #008080;">244</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">245</span>         TypeIs = <span style="color: #800080;">45</span><span style="color: #000000;">,
</span><span style="color: #008080;">246</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">247</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">248</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 赋值运算，如 (a = b)。
</span><span style="color: #008080;">249</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">250</span>         Assign = <span style="color: #800080;">46</span><span style="color: #000000;">,
</span><span style="color: #008080;">251</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">252</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">253</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 表达式块。
</span><span style="color: #008080;">254</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">255</span>         Block = <span style="color: #800080;">47</span><span style="color: #000000;">,
</span><span style="color: #008080;">256</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">257</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">258</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 调试信息。
</span><span style="color: #008080;">259</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">260</span>         DebugInfo = <span style="color: #800080;">48</span><span style="color: #000000;">,
</span><span style="color: #008080;">261</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">262</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">263</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 一元递减运算，如 C# 和 Visual Basic 中的 (a - 1)。 不应就地修改 a 对象。
</span><span style="color: #008080;">264</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">265</span>         Decrement = <span style="color: #800080;">49</span><span style="color: #000000;">,
</span><span style="color: #008080;">266</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">267</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">268</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 动态操作。
</span><span style="color: #008080;">269</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">270</span>         Dynamic = <span style="color: #800080;">50</span><span style="color: #000000;">,
</span><span style="color: #008080;">271</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">272</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">273</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 默认值。
</span><span style="color: #008080;">274</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">275</span>         Default = <span style="color: #800080;">51</span><span style="color: #000000;">,
</span><span style="color: #008080;">276</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">277</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">278</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 扩展表达式。
</span><span style="color: #008080;">279</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">280</span>         Extension = <span style="color: #800080;">52</span><span style="color: #000000;">,
</span><span style="color: #008080;">281</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">282</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">283</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> &ldquo;跳转&rdquo;表达式，如 C# 中的 goto Label 或 Visual Basic 中的 GoTo Label。
</span><span style="color: #008080;">284</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">285</span>         Goto = <span style="color: #800080;">53</span><span style="color: #000000;">,
</span><span style="color: #008080;">286</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">287</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">288</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 一元递增运算，如 C# 和 Visual Basic 中的 (a + 1)。 不应就地修改 a 对象。
</span><span style="color: #008080;">289</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">290</span>         Increment = <span style="color: #800080;">54</span><span style="color: #000000;">,
</span><span style="color: #008080;">291</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">292</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">293</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 索引运算或访问使用参数的属性的运算。
</span><span style="color: #008080;">294</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">295</span>         Index = <span style="color: #800080;">55</span><span style="color: #000000;">,
</span><span style="color: #008080;">296</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">297</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">298</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 标签。
</span><span style="color: #008080;">299</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">300</span>         Label = <span style="color: #800080;">56</span><span style="color: #000000;">,
</span><span style="color: #008080;">301</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">302</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">303</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 运行时变量的列表。 有关更多信息，请参见 System.Linq.Expressions.RuntimeVariablesExpression。
</span><span style="color: #008080;">304</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">305</span>         RuntimeVariables = <span style="color: #800080;">57</span><span style="color: #000000;">,
</span><span style="color: #008080;">306</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">307</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">308</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 循环，如 for 或 while。
</span><span style="color: #008080;">309</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">310</span>         Loop = <span style="color: #800080;">58</span><span style="color: #000000;">,
</span><span style="color: #008080;">311</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">312</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">313</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 多分支选择运算，如 C# 中的 switch 或 Visual Basic 中的 Select Case。
</span><span style="color: #008080;">314</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">315</span>         Switch = <span style="color: #800080;">59</span><span style="color: #000000;">,
</span><span style="color: #008080;">316</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">317</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">318</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 引发异常的运算，如 throw new Exception()。
</span><span style="color: #008080;">319</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">320</span>         Throw = <span style="color: #800080;">60</span><span style="color: #000000;">,
</span><span style="color: #008080;">321</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">322</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">323</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> try-catch 表达式。
</span><span style="color: #008080;">324</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">325</span>         Try = <span style="color: #800080;">61</span><span style="color: #000000;">,
</span><span style="color: #008080;">326</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">327</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">328</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 取消装箱值类型运算，如 MSIL 中的 unbox 和 unbox.any 指令。
</span><span style="color: #008080;">329</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">330</span>         Unbox = <span style="color: #800080;">62</span><span style="color: #000000;">,
</span><span style="color: #008080;">331</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">332</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">333</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 加法复合赋值运算，如 (a += b)，针对数值操作数，不进行溢出检查。
</span><span style="color: #008080;">334</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">335</span>         AddAssign = <span style="color: #800080;">63</span><span style="color: #000000;">,
</span><span style="color: #008080;">336</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">337</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">338</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 按位或逻辑 AND 复合赋值运算，如 C# 中的 (a &amp;= b)。
</span><span style="color: #008080;">339</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">340</span>         AndAssign = <span style="color: #800080;">64</span><span style="color: #000000;">,
</span><span style="color: #008080;">341</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">342</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">343</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 除法复合赋值运算，如 (a /= b)，针对数值操作数。
</span><span style="color: #008080;">344</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">345</span>         DivideAssign = <span style="color: #800080;">65</span><span style="color: #000000;">,
</span><span style="color: #008080;">346</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">347</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">348</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 按位或逻辑 XOR 复合赋值运算，如 C# 中的 (a ^= b)。
</span><span style="color: #008080;">349</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">350</span>         ExclusiveOrAssign = <span style="color: #800080;">66</span><span style="color: #000000;">,
</span><span style="color: #008080;">351</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">352</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">353</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 按位左移复合赋值运算，如 (a </span><span style="color: #808080;">&lt;&lt;= b)。
</span><span style="color: #008080;">354</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">355</span>         LeftShiftAssign = <span style="color: #800080;">67</span><span style="color: #000000;">,
</span><span style="color: #008080;">356</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">357</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">358</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 算术余数复合赋值运算，如 C# 中的 (a %= b)。
</span><span style="color: #008080;">359</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">360</span>         ModuloAssign = <span style="color: #800080;">68</span><span style="color: #000000;">,
</span><span style="color: #008080;">361</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">362</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">363</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 乘法复合赋值运算，如 (a *= b)，针对数值操作数，不进行溢出检查。
</span><span style="color: #008080;">364</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">365</span>         MultiplyAssign = <span style="color: #800080;">69</span><span style="color: #000000;">,
</span><span style="color: #008080;">366</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">367</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">368</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 按位或逻辑 OR 复合赋值运算，如 C# 中的 (a |= b)。
</span><span style="color: #008080;">369</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">370</span>         OrAssign = <span style="color: #800080;">70</span><span style="color: #000000;">,
</span><span style="color: #008080;">371</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">372</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">373</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 对某个数字进行幂运算的复合赋值运算，如 Visual Basic 中的 (a ^= b)。
</span><span style="color: #008080;">374</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">375</span>         PowerAssign = <span style="color: #800080;">71</span><span style="color: #000000;">,
</span><span style="color: #008080;">376</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">377</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">378</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 按位右移复合赋值运算，如 (a &gt;&gt;= b)。
</span><span style="color: #008080;">379</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">380</span>         RightShiftAssign = <span style="color: #800080;">72</span><span style="color: #000000;">,
</span><span style="color: #008080;">381</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">382</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">383</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 减法复合赋值运算，如 (a -= b)，针对数值操作数，不进行溢出检查。
</span><span style="color: #008080;">384</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">385</span>         SubtractAssign = <span style="color: #800080;">73</span><span style="color: #000000;">,
</span><span style="color: #008080;">386</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">387</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">388</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 加法复合赋值运算，如 (a += b)，针对数值操作数，进行溢出检查。
</span><span style="color: #008080;">389</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">390</span>         AddAssignChecked = <span style="color: #800080;">74</span><span style="color: #000000;">,
</span><span style="color: #008080;">391</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">392</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">393</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 乘法复合赋值运算，如 (a *= b)，针对数值操作数，进行溢出检查。
</span><span style="color: #008080;">394</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">395</span>         MultiplyAssignChecked = <span style="color: #800080;">75</span><span style="color: #000000;">,
</span><span style="color: #008080;">396</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">397</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">398</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 减法复合赋值运算，如 (a -= b)，针对数值操作数，进行溢出检查。
</span><span style="color: #008080;">399</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">400</span>         SubtractAssignChecked = <span style="color: #800080;">76</span><span style="color: #000000;">,
</span><span style="color: #008080;">401</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">402</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">403</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 一元前缀递增，如 (++a)。 应就地修改 a 对象。
</span><span style="color: #008080;">404</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">405</span>         PreIncrementAssign = <span style="color: #800080;">77</span><span style="color: #000000;">,
</span><span style="color: #008080;">406</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">407</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">408</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 一元前缀递减，如 (--a)。 应就地修改 a 对象。
</span><span style="color: #008080;">409</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">410</span>         PreDecrementAssign = <span style="color: #800080;">78</span><span style="color: #000000;">,
</span><span style="color: #008080;">411</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">412</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">413</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 一元后缀递增，如 (a++)。 应就地修改 a 对象。
</span><span style="color: #008080;">414</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">415</span>         PostIncrementAssign = <span style="color: #800080;">79</span><span style="color: #000000;">,
</span><span style="color: #008080;">416</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">417</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">418</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 一元后缀递减，如 (a--)。 应就地修改 a 对象。
</span><span style="color: #008080;">419</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">420</span>         PostDecrementAssign = <span style="color: #800080;">80</span><span style="color: #000000;">,
</span><span style="color: #008080;">421</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">422</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">423</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 确切类型测试。
</span><span style="color: #008080;">424</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">425</span>         TypeEqual = <span style="color: #800080;">81</span><span style="color: #000000;">,
</span><span style="color: #008080;">426</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">427</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">428</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 二进制反码运算，如 C# 中的 (~a)。
</span><span style="color: #008080;">429</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">430</span>         OnesComplement = <span style="color: #800080;">82</span><span style="color: #000000;">,
</span><span style="color: #008080;">431</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">432</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">433</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> true 条件值。
</span><span style="color: #008080;">434</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">435</span>         IsTrue = <span style="color: #800080;">83</span><span style="color: #000000;">,
</span><span style="color: #008080;">436</span>         <span style="color: #008000;">//
</span><span style="color: #008080;">437</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">438</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> false 条件值。
</span><span style="color: #008080;">439</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">440</span>         IsFalse = <span style="color: #800080;">84</span><span style="color: #000000;">,
</span><span style="color: #008080;">441</span> <span style="color: #000000;">    }
</span><span style="color: #008080;">442</span> }</pre>
</div>
<span class="cnblogs_code_collapse">ExpressionType</span></div>
<p>&nbsp;</p>
<p><span style="line-height: 1.5;">在查询条件方面，要做的就是解析其中一些类型的表达式。</span></p>
<h3>常用表达式举例</h3>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810320491164.png" alt="" /></p>
<p>下面列举出常用的查询语句。</p>
<p>&nbsp;</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span>             System.Linq.Expressions.Expression&lt;Func&lt;Cat, <span style="color: #0000ff;">bool</span>&gt;&gt; pre = <span style="color: #0000ff;">null</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 2</span> 
<span style="color: #008080;"> 3</span>             pre = x =&gt; x.Price == <span style="color: #800080;">10</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 4</span>             pre = x =&gt; x.Price &lt; <span style="color: #800080;">10</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 5</span>             pre = x =&gt; x.Price &gt; <span style="color: #800080;">10</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 6</span>             pre = x =&gt; x.Price &lt; <span style="color: #800080;">10</span> || x.Price &gt; <span style="color: #800080;">20</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 7</span>             pre = x =&gt; <span style="color: #800080;">10</span> &lt; x.Price &amp;&amp; x.Price &lt; <span style="color: #800080;">20</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 8</span>             pre = x =&gt; x.KittyName.Contains(<span style="color: #800000;">"</span><span style="color: #800000;">2</span><span style="color: #800000;">"</span><span style="color: #000000;">);
</span><span style="color: #008080;"> 9</span>             pre = x =&gt; x.KittyName.StartsWith(<span style="color: #800000;">"</span><span style="color: #800000;">kitty</span><span style="color: #800000;">"</span><span style="color: #000000;">);
</span><span style="color: #008080;">10</span>             pre = x =&gt; x.KittyName.EndsWith(<span style="color: #800000;">"</span><span style="color: #800000;">2</span><span style="color: #800000;">"</span>);</pre>
</div>
<p>&nbsp;</p>
<h1>数据库文件的逻辑结构</h1>
<h2>块(Block)</h2>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810327211792.png" alt="" /></p>
<p>数据库文件中要保存所有的表(Table)信息、各个表(Table)的索引(Index)信息、各个索引下的Skip List结点(Skip List Node)信息、各个Skip List Node的key和value（这是所有的数据库记录对象所在的位置）信息和所有的数据库记录。我们为不同种类的的信息分别设计一个类型，称为XXXBlock，它们都继承抽象类型Block。我们还规定，不同类型的Block只能存放在相应类型的页里（只有<span style="color: red;">一个例外</span>）。这样似乎效率更高。</p>
<p>&nbsp;</p>
<p style="background: white;"><img src="http://images0.cnblogs.com/blog/383191/201507/111810362993932.png" alt="" /></p>
<p>&nbsp;</p>
<h3>文件链表</h3>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810427362598.png" alt="" /></p>
<p>一个数据库，会有多个表(Table)。数据库里的表的数量随时会增加减少。要想把多个表存储到文件里，以后还能全部读出来，最好使用链表结构。我们用TableBlock描述存储到数据库文件里的一个表(Table)。TableBlock是在文件中的一个链表结点，其NextPos是指向文件中的某个位置的指针。只要有NextPos，就可以反序列化出NextObj，也就是下一个TableBlock。我把这种在文件中存在的链表称为<span style="color: red;">文件链表</span>。以前所见所用的链表则是<span style="color: red;">内存链表</span>。</p>
<p>一个表里，会有多个索引(Index)，类似的，IndexBlock也是一个文件链表。</p>
<p>SkipListNodeBlock存储的是Skip List的一个结点，而Skip List的结点有Down和Right两个指针，所以SkipListNodeBlock要存储两个指向文件中某处位置的指针DownPos和RightPos。就是说，SkipListNodeBlock是一个扩展了的文件链表。</p>
<p>了解了这些概念，就可以继续设计了。</p>
<p>&nbsp;</p>
<h3>Block</h3>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810445968997.png" alt="" /></p>
<p>任何一个块，都必须知道自己应该存放到数据库文件的位置(ThisPos)。为了能够进行序列化和反序列化，都要有[Serializable]特性。为了控制序列化和反序列化过程，要实现ISerializable接口。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 2</span>     <span style="color: #808080;">///</span><span style="color: #008000;"> 存储到数据库文件的一块内容。
</span><span style="color: #008080;"> 3</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 4</span> <span style="color: #000000;">    [Serializable]
</span><span style="color: #008080;"> 5</span>     <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">abstract</span> <span style="color: #0000ff;">class</span><span style="color: #000000;"> Block : ISerializable
</span><span style="color: #008080;"> 6</span> <span style="color: #000000;">    {
</span><span style="color: #008080;"> 7</span> 
<span style="color: #008080;"> 8</span> <span style="color: #0000ff;">#if</span> DEBUG
<span style="color: #008080;"> 9</span> 
<span style="color: #008080;">10</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">11</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 创建新</span><span style="color: #808080;">&lt;see cref="Block"/&gt;</span><span style="color: #008000;">时应设置其</span><span style="color: #808080;">&lt;see cref="Block.blockID"/&gt;</span><span style="color: #008000;">为计数器，并增长此计数器值。
</span><span style="color: #008080;">12</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">13</span>         <span style="color: #0000ff;">internal</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">long</span> IDCounter = <span style="color: #800080;">0</span><span style="color: #000000;">;
</span><span style="color: #008080;">14</span> 
<span style="color: #008080;">15</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">16</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 用于给此块标记一个编号，仅为便于调试之用。
</span><span style="color: #008080;">17</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">18</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">long</span><span style="color: #000000;"> blockID;
</span><span style="color: #008080;">19</span> <span style="color: #0000ff;">#endif</span>
<span style="color: #008080;">20</span> 
<span style="color: #008080;">21</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">22</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 此对象自身在数据库文件中的位置。为0时说明尚未指定位置。只有</span><span style="color: #808080;">&lt;see cref="DBHeaderBlock"/&gt;</span><span style="color: #008000;">的位置才应该为0。
</span><span style="color: #008080;">23</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">请注意在读写时设定此值。</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">24</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">25</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">long</span> ThisPos { <span style="color: #0000ff;">get</span>; <span style="color: #0000ff;">set</span><span style="color: #000000;">; }
</span><span style="color: #008080;">26</span> 
<span style="color: #008080;">27</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">28</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 存储到数据库文件的一块内容。
</span><span style="color: #008080;">29</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">30</span>         <span style="color: #0000ff;">public</span><span style="color: #000000;"> Block()
</span><span style="color: #008080;">31</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">32</span> <span style="color: #0000ff;">#if</span> DEBUG
<span style="color: #008080;">33</span>             <span style="color: #0000ff;">this</span>.blockID = IDCounter++<span style="color: #000000;">;
</span><span style="color: #008080;">34</span> <span style="color: #0000ff;">#endif</span>
<span style="color: #008080;">35</span>             BlockCache.AddFloatingBlock(<span style="color: #0000ff;">this</span><span style="color: #000000;">);
</span><span style="color: #008080;">36</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">37</span> 
<span style="color: #008080;">38</span> <span style="color: #0000ff;">#if</span> DEBUG
<span style="color: #008080;">39</span>         <span style="color: #0000ff;">const</span> <span style="color: #0000ff;">string</span> strBlockID = <span style="color: #800000;">""</span><span style="color: #000000;">;
</span><span style="color: #008080;">40</span> <span style="color: #0000ff;">#endif</span>
<span style="color: #008080;">41</span> 
<span style="color: #008080;">42</span>         <span style="color: #0000ff;">#region</span> ISerializable 成员
<span style="color: #008080;">43</span> 
<span style="color: #008080;">44</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">45</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 序列化时系统会调用此方法。
</span><span style="color: #008080;">46</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">47</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="info"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">48</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="context"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">49</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">virtual</span> <span style="color: #0000ff;">void</span><span style="color: #000000;"> GetObjectData(SerializationInfo info, StreamingContext context)
</span><span style="color: #008080;">50</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">51</span> <span style="color: #0000ff;">#if</span> DEBUG
<span style="color: #008080;">52</span>             info.AddValue(strBlockID, <span style="color: #0000ff;">this</span><span style="color: #000000;">.blockID);
</span><span style="color: #008080;">53</span> <span style="color: #0000ff;">#endif</span>
<span style="color: #008080;">54</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">55</span> 
<span style="color: #008080;">56</span>         <span style="color: #0000ff;">#endregion</span>
<span style="color: #008080;">57</span> 
<span style="color: #008080;">58</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">59</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> BinaryFormatter会通过调用此方法来反序列化此块。
</span><span style="color: #008080;">60</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">61</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="info"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">62</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="context"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">63</span>         <span style="color: #0000ff;">protected</span><span style="color: #000000;"> Block(SerializationInfo info, StreamingContext context)
</span><span style="color: #008080;">64</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">65</span> <span style="color: #0000ff;">#if</span> DEBUG
<span style="color: #008080;">66</span>             <span style="color: #0000ff;">this</span>.blockID =<span style="color: #000000;"> info.GetInt64(strBlockID);
</span><span style="color: #008080;">67</span> <span style="color: #0000ff;">#endif</span>
<span style="color: #008080;">68</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">69</span> 
<span style="color: #008080;">70</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">71</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 显示此块的信息，便于调试。
</span><span style="color: #008080;">72</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">73</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;">74</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">string</span><span style="color: #000000;"> ToString()
</span><span style="color: #008080;">75</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">76</span> <span style="color: #0000ff;">#if</span> DEBUG
<span style="color: #008080;">77</span>             <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">string</span>.Format(<span style="color: #800000;">"</span><span style="color: #800000;">{0}: ID:{1}, Pos: {2}</span><span style="color: #800000;">"</span>, <span style="color: #0000ff;">this</span>.GetType().Name, <span style="color: #0000ff;">this</span>.blockID, <span style="color: #0000ff;">this</span><span style="color: #000000;">.ThisPos);
</span><span style="color: #008080;">78</span> <span style="color: #0000ff;">#else</span>
<span style="color: #008080;">79</span>             <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">string</span>.Format(<span style="color: #800000;">"</span><span style="color: #800000;">{0}: Pos: {1}</span><span style="color: #800000;">"</span>, <span style="color: #0000ff;">this</span>.GetType().Name, <span style="color: #0000ff;">this</span><span style="color: #000000;">.ThisPos);
</span><span style="color: #008080;">80</span> <span style="color: #0000ff;">#endif</span>
<span style="color: #008080;">81</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">82</span> 
<span style="color: #008080;">83</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">84</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 安排所有文件指针。如果全部安排完毕，返回true，否则返回false。
</span><span style="color: #008080;">85</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">86</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;">87</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">abstract</span> <span style="color: #0000ff;">bool</span><span style="color: #000000;"> ArrangePos();
</span><span style="color: #008080;">88</span>     }</pre>
</div>
<p>&nbsp;</p>
<p>&nbsp;</p>
<h3>DBHeaderBlock</h3>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810462218467.png" alt="" /></p>
<p>这是整个数据库的头部，用于保存在数据库范围内的全局变量。它在整个数据库中只有一个，并且放在数据库的第一页（0~4095字节里）。</p>
<p>&nbsp;</p>
<h3>TableBlock</h3>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810482365651.png" alt="" /></p>
<p>TableBlock存放某种类型的表(Table)的信息，包括索引的头结点位置和下一个TableBlock的位置。前面说过，TableBlock是<span style="color: red;">内存链表</span>的一个结点。链表最好有个头结点，头结点不存储具有业务价值的数据，但是它会为编码提供方便。考虑到数据库的第一页只存放着一个DBHeaderBlock，我们就把TableBlock的头结点紧挨着放到DBHeaderBlock后面。这就是上面所说的<span style="color: red;">唯一的例外</span>。由于TableBlock的头结点不会移动位置，其序列化后的字节数也不会变，所以放这里是没有问题的。</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<h3>IndexBlock</h3>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810486897280.png" alt="" /></p>
<p>IndexBlock存储Table的一个索引。IndexBlock也是<span style="color: red;">内存链表</span>的一个结点。而它内部含有指向SkipListNodeBlock的指针，所以，实际上IndexBlock就充当了SkipList。</p>
<p>&nbsp;</p>
<h3>SkipListNodeBlock</h3>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810492369478.png" alt="" /></p>
<p>如前所述，这是一个扩展了的文件链表的结点。此结点的key和value都是指向实际数据的文件指针。如果直接保存实际数据，那么每个结点都保存一份完整的数据会造成很大的浪费。特别是value，如果value里有一个序列化了的图片，那是不可想象的。而且这样一来，所有的SkipListNodeBlock序列化的长度都是相同的。</p>
<p>&nbsp;</p>
<h3>DataBlock</h3>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810502839292.png" alt="" /></p>
<p>DataBlock也是文件链表的一个结点。由于某些数据库记录会很大（比如要存储一个<span style="color: black;"><span style="font-family: Lucida Console; font-size: 9pt; background-color: white;">System.Drawing.<span style="color: #2b91af;">Image</span></span>），一个页只有</span>4KB，无法放下。所以可能需要把一条记录划分为多个数据块，放到多个DataBlock里。也就是说，一个数据库记录是用一个链表保存的。</p>
<p>&nbsp;</p>
<h3>PageHeaderBlock</h3>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810511115406.png" alt="" /></p>
<p>为了以后管理页（申请新页、释放不再使用的页、申请一定长度的空闲空间），我们在每个页的起始位置都放置一个PageHeaderBlock，用来保存此页的状态（可以字节数等）。并且，每个页都包含一个指向下一相同类型的页的位置的文件指针。这样，所有存放TableBlock的页就成为一个文件链表，所有存放IndexBlock的页就成为另一个文件链表，所有存放SkipListNodeBlock的页也成为一个文件链表，所有存放DataBlock的页也一样。</p>
<p>另外，在删除某些记录后，有的页里存放的块可能为0，这时就成为一个空白页(Empty Page)，所以我们还要把这些空白页串联成一个文件链表。</p>
<p>总之，文件里的链表关系无处不在。</p>
<p>&nbsp;</p>
<h2>块(Block)在数据库文件里</h2>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111810514394307.png" alt="" /></p>
<p>下面是我画的一个数据库文件的逻辑上的结构图。它展示了各种类型的块在数据库文件里的生存状态。</p>
<p>首先，刚刚创建一个数据库文件时，文件里是这样的：</p>
<p style="background: white;"><img src="http://images0.cnblogs.com/blog/383191/201507/111811010028184.png" alt="" /></p>
<p>当前只有一个DBHeaderBlock和一个TableBlock（作为头结点）。</p>
<p>注意：此时我们忽略"页"这个概念，所以在每个页最开始处的PageHeaderBlock就不考虑了。</p>
<p>之后我们Insert一条记录，这会在数据库里新建一个表及其索引信息，然后插入此记录。指向完毕后，数据库文件里就是这样的。</p>
<p style="background: white;"><img src="http://images0.cnblogs.com/blog/383191/201507/111811189713329.png" alt="" /></p>
<p>之前的TableBlock头结点指向了新建的TableBlock的位置，新建的TableBlock创建了自己的索引。</p>
<p>索引有两个结点，上面的那个是索引的头结点，其不包含有业务价值的信息，只指向下一个索引结点。</p>
<p>下一个索引结点是第一个有业务意义的索引结点，也是一个存在于文件中的SkipList，它有自己的一群SkipListNodeBlock。在插入第一条记录前，这群SkipListNodeBlock只有竖直方向的那5个（实际上我在数据库文件里设定的是32个，不过没法画那么多，就用5个指代了）。</p>
<p>现在表和索引创建完毕，开始插入第一条记录。这会随机创建若干个（这里是2个）SkipListNodeBlock（这是Skip List数据结构的特性，具体请参考<a href="https://en.wikipedia.org/wiki/Skip_list">维基百科</a>。这两个SkipListNodeBlock的keyPos和valuePos都指向了key和value所在的DataBlock的位置。用于存储value的DataBlock有2个结点，说明value（数据库记录序列化后的字节数）比较大，一个页占不下。</p>
<p>这就是我们期望的情况。为了实现这种文件链表，还需要后续一些遍历操作。我们将结合事务来完成它。</p>
<p>如果你感兴趣，下面是继续插入第二条记录后的情况：</p>
<p style="background: white;"><img src="http://images0.cnblogs.com/blog/383191/201507/111811304083134.png" alt="" /></p>
<p>注：为了避免图纸太乱，我只画出了最下面的K1, V1和K2, V2的指针指向DataBlock。实际上，各个K1, V1和K2, V2都是指向DataBlock的。</p>
<p>&nbsp;</p>
<h2>为块(Block)安排其在文件中的位置</h2>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111811314864704.png" alt="" /></p>
<h3>根据依赖关系依次分配</h3>
<p>新创建一个Block时，其在数据库文件中的位置(Block.ThisPos)都没有指定，那么在其它Block中指向它的那些字段/属性值就无法确定。我们通过两个步骤来解决此问题。</p>
<p>首先，我们给每个文件链表结点的NextPos都配备一个对应的NextObj。就是说，新创建的Block虽然在文件链表方面还没有安排好指针，但是在内存链表方面已经安排好了。</p>
<p>然后，等所需的所有Block都创建完毕，遍历这些Block，那些在内存链表中处于最后一个的结点，其字段/属性值不依赖其它Block的位置，因此可以直接为其分配好在文件里的位置和空间。之后再次遍历这些Block，那些依赖最后一个结点的结点，此时也就可以为其设置字段/属性值了。以此类推，多次遍历，直到所有Block的字段/属性值都设置完毕。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span>             <span style="color: #008000;">//</span><span style="color: #008000;"> 给所有的块安排数据库文件中的位置。</span>
<span style="color: #008080;"> 2</span>             List&lt;Block&gt; arrangedBlocks = <span style="color: #0000ff;">new</span> List&lt;Block&gt;<span style="color: #000000;">();
</span><span style="color: #008080;"> 3</span>             <span style="color: #0000ff;">while</span> (arrangedBlocks.Count &lt; <span style="color: #0000ff;">this</span><span style="color: #000000;">.blockList.Count)
</span><span style="color: #008080;"> 4</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 5</span>                 <span style="color: #0000ff;">for</span> (<span style="color: #0000ff;">int</span> i = <span style="color: #0000ff;">this</span>.blockList.Count - <span style="color: #800080;">1</span>; i &gt;= <span style="color: #800080;">0</span>; i--)<span style="color: #008000;">//</span><span style="color: #008000;"> 后加入列表的先处理。</span>
<span style="color: #008080;"> 6</span> <span style="color: #000000;">                {
</span><span style="color: #008080;"> 7</span>                     Block block = <span style="color: #0000ff;">this</span><span style="color: #000000;">.blockList[i];
</span><span style="color: #008080;"> 8</span>                     <span style="color: #0000ff;">if</span> (arrangedBlocks.Contains(block)) { <span style="color: #0000ff;">continue</span><span style="color: #000000;">; }
</span><span style="color: #008080;"> 9</span>                     <span style="color: #0000ff;">bool</span> done =<span style="color: #000000;"> block.ArrangePos();
</span><span style="color: #008080;">10</span>                     <span style="color: #0000ff;">if</span><span style="color: #000000;"> (done)
</span><span style="color: #008080;">11</span> <span style="color: #000000;">                    {
</span><span style="color: #008080;">12</span>                         <span style="color: #0000ff;">if</span> (block.ThisPos == <span style="color: #800080;">0</span><span style="color: #000000;">)
</span><span style="color: #008080;">13</span> <span style="color: #000000;">                        {
</span><span style="color: #008080;">14</span>                             <span style="color: #0000ff;">byte</span>[] bytes =<span style="color: #000000;"> block.ToBytes();
</span><span style="color: #008080;">15</span>                             <span style="color: #0000ff;">if</span> (bytes.Length &gt;<span style="color: #000000;"> Consts.maxAvailableSpaceInPage)
</span><span style="color: #008080;">16</span>                             { <span style="color: #0000ff;">throw</span> <span style="color: #0000ff;">new</span> Exception(<span style="color: #800000;">"</span><span style="color: #800000;">Block size is toooo large!</span><span style="color: #800000;">"</span><span style="color: #000000;">); }
</span><span style="color: #008080;">17</span>                             AllocPageTypes pageType =<span style="color: #000000;"> block.BelongedPageType();
</span><span style="color: #008080;">18</span>                             AllocatedSpace space = <span style="color: #0000ff;">this</span><span style="color: #000000;">.fileDBContext.Alloc(bytes.LongLength, pageType);
</span><span style="color: #008080;">19</span>                             block.ThisPos =<span style="color: #000000;"> space.position;
</span><span style="color: #008080;">20</span> <span style="color: #000000;">                        }
</span><span style="color: #008080;">21</span> 
<span style="color: #008080;">22</span> <span style="color: #000000;">                        arrangedBlocks.Add(block);
</span><span style="color: #008080;">23</span> <span style="color: #000000;">                    }
</span><span style="color: #008080;">24</span> <span style="color: #000000;">                }
</span><span style="color: #008080;">25</span>             }</pre>
</div>
<p>&nbsp;</p>
<p>我们要为不同类型的块执行各自的字段/属性值的设置方法，通过继承Block基类的<span style="color: blue;"><span style="font-family: Lucida Console; font-size: 9pt; background-color: white;">abstract<span style="color: black;"> <span style="color: blue;">bool<span style="color: black;"> ArrangePos<span style="color: yellowgreen;">()<span style="color: black;">;</span></span></span></span></span></span></span>来实现。实际上，只要添加到blockList里的顺序得当，只需一次遍历即可完成所有的自动/属性值的设置。&nbsp;</p>
<h3>DBHeaderBlock</h3>
<p>此类型的字段/属性值不依赖其它任何Block，永远都是实时分配完成的。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;">1</span>         <span style="color: #0000ff;">internal</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">bool</span><span style="color: #000000;"> ArrangePos()
</span><span style="color: #008080;">2</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">3</span>             <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">true</span>;<span style="color: #008000;">//</span><span style="color: #008000;"> 此类型比较特殊，应该在更新时立即指定各项文件指针。</span>
<span style="color: #008080;">4</span>         }</pre>
</div>
<p>&nbsp;</p>
<h3>TableBlock</h3>
<p>作为头结点的那个TableBlock不含索引，因此其字段/属性值不需设置。其它TableBlock则需要保存索引头结点的位置。</p>
<p>作为链表的最后一个结点的那个TableBlock的字段/属性值不依赖其它TableBlock的位置，其它TableBLock则需要其下一个TableBlock的位置。这一规则对每个文件链表都适用。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">bool</span><span style="color: #000000;"> ArrangePos()
</span><span style="color: #008080;"> 2</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 3</span>             <span style="color: #0000ff;">bool</span> allArranged = <span style="color: #0000ff;">true</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 4</span> 
<span style="color: #008080;"> 5</span>             <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span>.IndexBlockHead != <span style="color: #0000ff;">null</span>)<span style="color: #008000;">//</span><span style="color: #008000;"> 如果IndexBlockHead == null，则说明此块为TableBlock的头结点。头结点是不需要持有索引块的。</span>
<span style="color: #008080;"> 6</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 7</span>                 <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span>.IndexBlockHead.ThisPos != <span style="color: #800080;">0</span><span style="color: #000000;">)
</span><span style="color: #008080;"> 8</span>                 { <span style="color: #0000ff;">this</span>.IndexBlockHeadPos = <span style="color: #0000ff;">this</span><span style="color: #000000;">.IndexBlockHead.ThisPos; }
</span><span style="color: #008080;"> 9</span>                 <span style="color: #0000ff;">else</span>
<span style="color: #008080;">10</span>                 { allArranged = <span style="color: #0000ff;">false</span><span style="color: #000000;">; }
</span><span style="color: #008080;">11</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">12</span> 
<span style="color: #008080;">13</span>             <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span>.NextObj != <span style="color: #0000ff;">null</span><span style="color: #000000;">)
</span><span style="color: #008080;">14</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">15</span>                 <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span>.NextObj.ThisPos != <span style="color: #800080;">0</span><span style="color: #000000;">)
</span><span style="color: #008080;">16</span>                 { <span style="color: #0000ff;">this</span>.NextPos = <span style="color: #0000ff;">this</span><span style="color: #000000;">.NextObj.ThisPos; }
</span><span style="color: #008080;">17</span>                 <span style="color: #0000ff;">else</span>
<span style="color: #008080;">18</span>                 { allArranged = <span style="color: #0000ff;">false</span><span style="color: #000000;">; }
</span><span style="color: #008080;">19</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">20</span> 
<span style="color: #008080;">21</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> allArranged;
</span><span style="color: #008080;">22</span>         }</pre>
</div>
<p>&nbsp;</p>
<h3>IndexBlock</h3>
<p>IndexBlock也是文件链表的一个结点，其字段/属性值的设置方式与TableBlock相似。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">bool</span><span style="color: #000000;"> ArrangePos()
</span><span style="color: #008080;"> 2</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 3</span>             <span style="color: #0000ff;">bool</span> allArranged = <span style="color: #0000ff;">true</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 4</span> 
<span style="color: #008080;"> 5</span>             <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span>.SkipListHeadNodes != <span style="color: #0000ff;">null</span>)<span style="color: #008000;">//</span><span style="color: #008000;"> 如果这里的SkipListHeadNodes == null，则说明此索引块是索引链表里的头结点。头结点是不需要SkipListHeadNodes有数据的。</span>
<span style="color: #008080;"> 6</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 7</span>                 <span style="color: #0000ff;">int</span> length = <span style="color: #0000ff;">this</span><span style="color: #000000;">.SkipListHeadNodes.Length;
</span><span style="color: #008080;"> 8</span>                 <span style="color: #0000ff;">if</span> (length == <span style="color: #800080;">0</span><span style="color: #000000;">)
</span><span style="color: #008080;"> 9</span>                 { <span style="color: #0000ff;">throw</span> <span style="color: #0000ff;">new</span> Exception(<span style="color: #800000;">"</span><span style="color: #800000;">SKip List's head nodes has 0 element!</span><span style="color: #800000;">"</span><span style="color: #000000;">); }
</span><span style="color: #008080;">10</span>                 <span style="color: #0000ff;">long</span> pos = <span style="color: #0000ff;">this</span>.SkipListHeadNodes[length - <span style="color: #800080;">1</span><span style="color: #000000;">].ThisPos;
</span><span style="color: #008080;">11</span>                 <span style="color: #0000ff;">if</span> (pos != <span style="color: #800080;">0</span><span style="color: #000000;">)
</span><span style="color: #008080;">12</span>                 { <span style="color: #0000ff;">this</span>.SkipListHeadNodePos =<span style="color: #000000;"> pos; }
</span><span style="color: #008080;">13</span>                 <span style="color: #0000ff;">else</span>
<span style="color: #008080;">14</span>                 { allArranged = <span style="color: #0000ff;">false</span><span style="color: #000000;">; }
</span><span style="color: #008080;">15</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">16</span> 
<span style="color: #008080;">17</span>             <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span>.SkipListTailNode != <span style="color: #0000ff;">null</span>)<span style="color: #008000;">//</span><span style="color: #008000;"> 如果这里的SkipListTailNodes == null，则说明此索引块是索引链表里的头结点。头结点是不需要SkipListTailNodes有数据的。</span>
<span style="color: #008080;">18</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">19</span>                 <span style="color: #0000ff;">long</span> pos = <span style="color: #0000ff;">this</span><span style="color: #000000;">.SkipListTailNode.ThisPos;
</span><span style="color: #008080;">20</span>                 <span style="color: #0000ff;">if</span> (pos != <span style="color: #800080;">0</span><span style="color: #000000;">)
</span><span style="color: #008080;">21</span>                 { <span style="color: #0000ff;">this</span>.SkipListTailNodePos =<span style="color: #000000;"> pos; }
</span><span style="color: #008080;">22</span>                 <span style="color: #0000ff;">else</span>
<span style="color: #008080;">23</span>                 { allArranged = <span style="color: #0000ff;">false</span><span style="color: #000000;">; }
</span><span style="color: #008080;">24</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">25</span> 
<span style="color: #008080;">26</span>             <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span>.NextObj != <span style="color: #0000ff;">null</span><span style="color: #000000;">)
</span><span style="color: #008080;">27</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">28</span>                 <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span>.NextObj.ThisPos != <span style="color: #800080;">0</span><span style="color: #000000;">)
</span><span style="color: #008080;">29</span>                 { <span style="color: #0000ff;">this</span>.NextPos = <span style="color: #0000ff;">this</span><span style="color: #000000;">.NextObj.ThisPos; }
</span><span style="color: #008080;">30</span>                 <span style="color: #0000ff;">else</span>
<span style="color: #008080;">31</span>                 { allArranged = <span style="color: #0000ff;">false</span><span style="color: #000000;">; }
</span><span style="color: #008080;">32</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">33</span> 
<span style="color: #008080;">34</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> allArranged;
</span><span style="color: #008080;">35</span>         }</pre>
</div>
<p>&nbsp;</p>
<h3>SkipListNodeBlock</h3>
<p>SkipListNodeBlock是扩展了的文件链表，关于指向下一结点的指针的处理与前面类似。作为头结点的SkipListNodeBlock的Key和Value都是null，是不依赖其它Block的。非头结点的SkipListNodeBlock的Key和Value则依赖保存着序列化后的Key和Value的DataBlock。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">bool</span><span style="color: #000000;"> ArrangePos()
</span><span style="color: #008080;"> 2</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 3</span>             <span style="color: #0000ff;">bool</span> allArranged = <span style="color: #0000ff;">true</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 4</span> 
<span style="color: #008080;"> 5</span>             <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span>.Key != <span style="color: #0000ff;">null</span><span style="color: #000000;">)
</span><span style="color: #008080;"> 6</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 7</span>                 <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span>.Key.ThisPos != <span style="color: #800080;">0</span><span style="color: #000000;">)
</span><span style="color: #008080;"> 8</span>                 { <span style="color: #0000ff;">this</span>.KeyPos = <span style="color: #0000ff;">this</span><span style="color: #000000;">.Key.ThisPos; }
</span><span style="color: #008080;"> 9</span>                 <span style="color: #0000ff;">else</span>
<span style="color: #008080;">10</span>                 { allArranged = <span style="color: #0000ff;">false</span><span style="color: #000000;">; }
</span><span style="color: #008080;">11</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">12</span> 
<span style="color: #008080;">13</span>             <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span>.Value != <span style="color: #0000ff;">null</span><span style="color: #000000;">)
</span><span style="color: #008080;">14</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">15</span>                 <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span>.Value[<span style="color: #800080;">0</span>].ThisPos != <span style="color: #800080;">0</span><span style="color: #000000;">)
</span><span style="color: #008080;">16</span>                 { <span style="color: #0000ff;">this</span>.ValuePos = <span style="color: #0000ff;">this</span>.Value[<span style="color: #800080;">0</span><span style="color: #000000;">].ThisPos; }
</span><span style="color: #008080;">17</span>                 <span style="color: #0000ff;">else</span>
<span style="color: #008080;">18</span>                 { allArranged = <span style="color: #0000ff;">false</span><span style="color: #000000;">; }
</span><span style="color: #008080;">19</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">20</span> 
<span style="color: #008080;">21</span>             <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span>.DownObj != <span style="color: #0000ff;">null</span>)<span style="color: #008000;">//</span><span style="color: #008000;"> 此结点不是最下方的结点。</span>
<span style="color: #008080;">22</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">23</span>                 <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span>.DownObj.ThisPos != <span style="color: #800080;">0</span><span style="color: #000000;">)
</span><span style="color: #008080;">24</span>                 { <span style="color: #0000ff;">this</span>.DownPos = <span style="color: #0000ff;">this</span><span style="color: #000000;">.DownObj.ThisPos; }
</span><span style="color: #008080;">25</span>                 <span style="color: #0000ff;">else</span>
<span style="color: #008080;">26</span>                 { allArranged = <span style="color: #0000ff;">false</span><span style="color: #000000;">; }
</span><span style="color: #008080;">27</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">28</span> 
<span style="color: #008080;">29</span>             <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span>.RightObj != <span style="color: #0000ff;">null</span>)<span style="color: #008000;">//</span><span style="color: #008000;"> 此结点不是最右方的结点。</span>
<span style="color: #008080;">30</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">31</span>                 <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span>.RightObj.ThisPos != <span style="color: #800080;">0</span><span style="color: #000000;">)
</span><span style="color: #008080;">32</span>                 { <span style="color: #0000ff;">this</span>.RightPos = <span style="color: #0000ff;">this</span><span style="color: #000000;">.RightObj.ThisPos; }
</span><span style="color: #008080;">33</span>                 <span style="color: #0000ff;">else</span>
<span style="color: #008080;">34</span>                 { allArranged = <span style="color: #0000ff;">false</span><span style="color: #000000;">; }
</span><span style="color: #008080;">35</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">36</span> 
<span style="color: #008080;">37</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> allArranged;
</span><span style="color: #008080;">38</span>         }</pre>
</div>
<p>&nbsp;</p>
<h3>DataBlock</h3>
<p>DataBlock就是一个很单纯的文件链表结点。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">bool</span><span style="color: #000000;"> ArrangePos()
</span><span style="color: #008080;"> 2</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 3</span>             <span style="color: #0000ff;">bool</span> allArranged = <span style="color: #0000ff;">true</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 4</span> 
<span style="color: #008080;"> 5</span>             <span style="color: #0000ff;">if</span> (NextObj != <span style="color: #0000ff;">null</span><span style="color: #000000;">)
</span><span style="color: #008080;"> 6</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 7</span>                 <span style="color: #0000ff;">if</span> (NextObj.ThisPos != <span style="color: #800080;">0</span><span style="color: #000000;">)
</span><span style="color: #008080;"> 8</span>                 { <span style="color: #0000ff;">this</span>.NextPos =<span style="color: #000000;"> NextObj.ThisPos; }
</span><span style="color: #008080;"> 9</span>                 <span style="color: #0000ff;">else</span>
<span style="color: #008080;">10</span>                 { allArranged = <span style="color: #0000ff;">false</span><span style="color: #000000;">; }
</span><span style="color: #008080;">11</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">12</span> 
<span style="color: #008080;">13</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> allArranged;
</span><span style="color: #008080;">14</span>         }</pre>
</div>
<p>&nbsp;</p>
<h3>PageHeaderBlock</h3>
<p>此类型只在创建新页和申请已有页空间时出现，不会参与上面各类Block的位置分配，而是在创建时就为其安排好NextPagePos等属性。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;">1</span>         <span style="color: #0000ff;">internal</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">bool</span><span style="color: #000000;"> ArrangePos()
</span><span style="color: #008080;">2</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">3</span>             <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">true</span>;<span style="color: #008000;">//</span><span style="color: #008000;"> 此类型比较特殊，应该在创建时就为其安排好NextPagePos等属性。</span>
<span style="color: #008080;">4</span>         }</pre>
</div>
<p>&nbsp;</p>
<h1>Demo和工具</h1>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111811324398248.png" alt="" /></p>
<p>为了方便调试和使用SharpFileDB，我做了下面这些工具和示例Demo。</p>
<h2>Demo：MyNote</h2>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111811329712919.png" alt="" /></p>
<p>这是一个非常简单的Demo，实现一个简单的便签列表，演示了如何使用SharpFileDB。虽然界面不好看，但是用作Demo也不应该有太多的无关代码。</p>
<p style="background: white;"><img src="http://images0.cnblogs.com/blog/383191/201507/111811360338915.png" alt="" /></p>
<p>&nbsp;</p>
<h2>查看数据库状态</h2>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111811368466501.png" alt="" /></p>
<p>为了能够直观地查看数据库的状态（包含哪些表、索引、数据记录），方便调试，这里顺便提供一个Winform程序"SharpFileDB Viewer"。这样可以看到数据库的几乎全部信息，调试起来就方便多了。</p>
<p>点击"Refresh"会重新加载所有的表、索引和记录信息，简单粗暴。</p>
<p style="background: white;"><img src="http://images0.cnblogs.com/blog/383191/201507/111811383611771.png" alt="" /></p>
<p>&nbsp;</p>
<p>点击"Skip Lists"会把数据库里所有表的所有索引的所有结点和所有数据都画到BMP图片上。比如上面看到的MyNote.db的索引分别情况如下图所示。</p>
<p style="background: white;"><img src="http://images0.cnblogs.com/blog/383191/201507/111811400187699.png" alt="" /></p>
<p>此图绘制了Note表的唯一一个索引和表里的全部记录（共10条）。</p>
<p>由于为Skip List指定了最大层数为8，且现在数据库里只有10条记录，所以图示上方比较空旷。</p>
<p>下面是局部视图，看得比较清楚。</p>
<p style="background: white;"><img src="http://images0.cnblogs.com/blog/383191/201507/111811419082154.png" alt="" /></p>
<p>&nbsp;</p>
<p>点击"Blocks"会把数据库各个页上包含的各个块的占用情况画到多个宽为4096高为100的BMP图片上。例如下面6张图是添加了一个表、一个索引和一条记录后，数据库文件的全部6个页的情况。</p>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111811422831340.png" alt="" /></p>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111811430646169.png" alt="" /></p>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111811433612910.png" alt="" /></p>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111811437362095.png" alt="" /></p>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111811441111281.png" alt="" /></p>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111811448936110.png" alt="" /></p>
<p>如上图所示，每一页头部都含有一个PageHeaderBlock，用浅绿色表示。</p>
<p>第一页还有一个数据库头部DBHeaderBlock（用青紫色表示）和一个TableBlock头结点（用橙色表示）。第六页也有一个TableBlock，它代表目前数据库的唯一一个表。Table头结点的TableType属性是空的，所以其长度比第六页的TableBlock要短。这些图片是完全准确地依照各个Block在数据库文件中存储的位置和长度绘制的。</p>
<p>第二页是2个DataBlock，用金色表示。（DataBlock里存放的才是有业务价值的数据，所以是金子的颜色）</p>
<p>第三、第四页存放的都是SkipListNodeBlock（用绿色表示）。可见索引是比较占地方的。</p>
<p>第五页存放了两个IndexBlock（其中一个是IndexBlock的头结点），用灰色表示。</p>
<p>这些图片就比前面手工画的草图好看多了。</p>
<p>以后有时间我把这些图片画到Form上，当鼠标停留在一个块上时，会显示此块的具体信息（如位置、长度、类型、相关联的块等），那就更方便监视数据库的状态了。</p>
<p>&nbsp;</p>
<p>后续我会根据需要增加显示更多信息的功能。</p>
<p>&nbsp;</p>
<h2>可视化的数据库设计器</h2>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111811453938023.png" alt="" /></p>
<p>如果能像MSSQLServer那样用可视化的方法创建自定义表，做成自己的数据库，那就最好了。所以我写了这样一个可视化的数据库设计器，你可以用可视化方式设计你的数据库，然后一键生成所有代码。</p>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111811480647308.png" alt="" /></p>
<p>&nbsp;</p>
<h1>规则/坑</h1>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111811488933421.png" alt="" /></p>
<p>各种各样的库、工具包都有一些隐晦的规则，一旦触及就会引发各种奇怪的问题。这种规则其实就是坑。SharpFileDB尽可能不设坑或不设深坑。那些实在无法阻挡的坑，我就让它浅浅地、明显地存在着，即使掉进去了也能马上跳出来。另外，通过使用可视化的设计器，还可以自动避免掉坑里，因为设计器在生成代码前是会提醒你是否已经一脚进坑了。</p>
<p>SharpFileDB有如下几条规则（几个坑）你需要知道：</p>
<h2>继承Table的表类型序列化后不能太大</h2>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111811491741635.png" alt="" /></p>
<p>你设计的Table类型序列化后的长度不能超过Int32.MaxValue个字节（= 2097151KB = 2047MB &asymp;2GB）。这个坑够浅了吧。如果一个对象的能占据2GB，那早就不该用SharpFileDB了。所以这个坑应该不会有人踩到。</p>
<p>&nbsp;</p>
<h2>只能在属性上添加索引</h2>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111811497521292.png" alt="" /></p>
<p>索引必须加在属性上，否则无效。为什么呢？因为我在实现SharpFileDB的时候只检测了表类型的各个<span style="color: #2b91af; font-family: Lucida Console; font-size: 9pt; background-color: white;">PropertyInfo</span>是否附带[TableIndex]特性。我想，这算是人为赋予属性的一种荣誉吧。</p>
<p>&nbsp;</p>
<p>目前为止，这两个坑算是最深的了。但愿它们的恶劣影响不大。</p>
<p>&nbsp;</p>
<p>剩下的几个坑是给SharpFileDB开发者的（目前就是我）。我会在各种可能发现bug的地方直接抛出异常。等这些throw消停了，就说明SharpFileDB稳定了。我再想个办法把这些难看的throw收拾收拾。</p>
<p>&nbsp;</p>
<h1>总结</h1>
<p><img src="http://images0.cnblogs.com/blog/383191/201507/111811504716905.png" alt="" /></p>
<p>现在的SharpFileDB很多方面（编号、分页、索引、查询、块）都受到LiteDB的启示，再次表示感谢。</p>
<p>您可以到我的Github上浏览此项目。</p>
<p>也许我做的这个SharpFileDB很幼稚，不过我相信它可以用于实践，我也尽量写了注释、提供了最简单的使用方法，还提供了Demo。还是那句话，欢迎您指出我的代码有哪些不足，我应该学习补充哪些知识，但请不要无意义地谩骂和人身攻击。</p>
<p>&nbsp;</p>
<p>PS：我国大多数县的人口为几万到几十万。目前，县里各种政府部门急需实现信息化网络化办公办事，但他们一般用不起那种月薪上万的开发者和高端软件公司。我注意到，一个县级政府部门日常应对的人群数量就是<span style="color: #ff0000;">万人</span>左右，甚至常常是千人左右。所以他们不需要太高端复杂的系统设计，用支持万人级别的数据库就可以了。另一方面，初级开发者也不能充分利用那些看似高端复杂的数据库的优势。做个小型系统而已，还是简单一点好。</p>
<p>所以我就想做这样一个小型文件数据库，我相信这会帮助很多人。能以己所学惠及大众，才是我们的价值所在。</p>

