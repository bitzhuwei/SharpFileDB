# SharpFileDB
SharpFileDB is a micro database library that uses no SQL, files as storage form and totally C# to implement a CRUD system. 
SharpFileDB是一个纯C#的无SQL的支持CRUD的小型文件数据库，目标是支持万人级别的应用程序。
<p style="text-align: center;"><span style="font-size: 16pt;"><strong>小型文件数据库 (a file database for small apps) SharpFileDB </strong></span></p>
<p>For english version of this article, please click <a href="http://www.cnblogs.com/bitzhuwei/p/SharpFileDB-eng.html" target="_blank">here</a>.</p>
<p>我并不擅长数据库，如有不当之处，请多多指教。</p>
<p>本文参考了（<a href="http://www.cnblogs.com/gaochundong/archive/2013/04/24/csharp_file_database.html" target="_blank">http://www.cnblogs.com/gaochundong/archive/2013/04/24/csharp_file_database.html</a>），在此表示感谢！</p>
<h1>目标(Goal)</h1>
<p><img src="http://images0.cnblogs.com/blog/383191/201506/220145437206232.png" alt="" /></p>
<p>我决定做一个以<span style="color: #ff0000;">支持小型应用（万人级别）</span>为目标的数据库。</p>
<p>既然是小型的数据库，那么最好<span style="color: #ff0000;">不要依赖其它驱动、工具包</span>，免得拖泥带水难以实施。</p>
<p><span style="color: #ff0000;">完全用C#编写</span>成DLL，易学易用。</p>
<p>支持<span style="color: #ff0000;">CRUD</span>（增加(Create)、读取(Retrieve)、更新(Update)和删除(Delete)）。</p>
<p><span style="color: #ff0000;">不使用SQL</span>，客观原因我不擅长SQL，主观原因我不喜欢SQL，情景原因没有必要。</p>
<p>直接<span style="color: #ff0000;">用文本文件或二进制文件存储数据</span>。开发时用文本文件，便于调试；发布时用二进制文件，比较安全。</p>
<p>简单来说，就是纯C#、小型、无SQL。此类库就命名为<strong>SharpFileDB</strong>。</p>
<p>为了便于共同开发，我把这个项目放到<a href="https://github.com/bitzhuwei/SharpFileDB/">Github</a>上，并且所有类库代码的注释都是中英文双语的。中文便于理解，英文便于今后国际化。也许我想的太多了。</p>
<h1>设计草图(sketch)</h1>
<h2>使用场景(User Scene)</h2>
<p>SharpFileDB库的典型使用场景如下。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span>                 <span style="color: #008000;">//</span><span style="color: #008000;"> common cases to use SharpFileDB.</span>
<span style="color: #008080;"> 2</span>                 FileDBContext db = <span style="color: #0000ff;">new</span><span style="color: #000000;"> FileDBContext();
</span><span style="color: #008080;"> 3</span> 
<span style="color: #008080;"> 4</span>                 Cat cat = <span style="color: #0000ff;">new</span><span style="color: #000000;"> Cat();
</span><span style="color: #008080;"> 5</span>                 cat.Name = <span style="color: #800000;">"</span><span style="color: #800000;">xiao xiao bai</span><span style="color: #800000;">"</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 6</span> <span style="color: #000000;">                db.Create(cat);
</span><span style="color: #008080;"> 7</span> 
<span style="color: #008080;"> 8</span>                 Predicate&lt;Cat&gt; pre = <span style="color: #0000ff;">new</span> Predicate&lt;Cat&gt;(x =&gt; x.Name == <span style="color: #800000;">"</span><span style="color: #800000;">xiao xiao bai</span><span style="color: #800000;">"</span><span style="color: #000000;">);
</span><span style="color: #008080;"> 9</span>                 IList&lt;Cat&gt; cats =<span style="color: #000000;"> db.Retrieve(pre);
</span><span style="color: #008080;">10</span> 
<span style="color: #008080;">11</span>                 cat.Name = <span style="color: #800000;">"</span><span style="color: #800000;">xiao bai</span><span style="color: #800000;">"</span><span style="color: #000000;">;
</span><span style="color: #008080;">12</span> <span style="color: #000000;">                db.Update(cat);
</span><span style="color: #008080;">13</span> 
<span style="color: #008080;">14</span>                 db.Delete(cat);</pre>
</div>
<p><span style="line-height: 1.5;">这个场景里包含了创建数据库和使用CRUD操作的情形。</span></p>
<p>我们就从这个使用场景开始设计出第一版最简单的一个文件数据库。</p>
<h1>核心概念(Core Concepts)</h1>
<p>如下图所示，数据库有三个核心的东西：数据库上下文，就是数据库本身，能够执行CRUD操作；表，在这里是一个个文件，用一个FileObject类型表示一个表；持久化工具，实现CRUD操作，把信息存储到数据库中。</p>
<p><img src="http://images0.cnblogs.com/blog2015/383191/201506/222327388616950.png" alt="" /></p>
<p>&nbsp;</p>
<h2>表vs类型(Table vs Type)</h2>
<p>为方便叙述，下面我们以Cat为例进行说明。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 2</span>     <span style="color: #808080;">///</span><span style="color: #008000;"> demo file object
</span><span style="color: #008080;"> 3</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 4</span>     <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">class</span><span style="color: #000000;"> Cat : FileObject
</span><span style="color: #008080;"> 5</span> <span style="color: #000000;">    {
</span><span style="color: #008080;"> 6</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">string</span> Name { <span style="color: #0000ff;">get</span>; <span style="color: #0000ff;">set</span><span style="color: #000000;">; }
</span><span style="color: #008080;"> 7</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">int</span> Legs { <span style="color: #0000ff;">get</span>; <span style="color: #0000ff;">set</span><span style="color: #000000;">; }
</span><span style="color: #008080;"> 8</span> 
<span style="color: #008080;"> 9</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">string</span><span style="color: #000000;"> ToString()
</span><span style="color: #008080;">10</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">11</span>             <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">string</span>.Format(<span style="color: #800000;">"</span><span style="color: #800000;">{0}, Name: {1}, Legs: {2}</span><span style="color: #800000;">"</span>, <span style="color: #0000ff;">base</span><span style="color: #000000;">.ToString(), Name, Legs);
</span><span style="color: #008080;">12</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">13</span>     }</pre>
</div>
<p><span style="line-height: 1.5;">Cat这个类型就等价于关系数据库里的一个Table。</span></p>
<p>Cat的一个实例，就等价于关系数据库的Table里的一条记录。</p>
<p>以后我们把这样的类型称为<span style="color: red;">表类型</span>。</p>
<h2>全局唯一的主键(global unique main key)</h2>
<p>类似关系数据库的主键，我们需要用全局唯一的Id来区分每个对象。每个表类型的实例都需要这样一个Id，那么我们就用一个abstract基类做这件事。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 2</span>     <span style="color: #808080;">///</span><span style="color: #008000;"> 可在文件数据库中使用CRUD操作的所有类型的基类。
</span><span style="color: #008080;"> 3</span>     <span style="color: #808080;">///</span><span style="color: #008000;"> Base class for all classed that can use CRUD in SharpFileDB.
</span><span style="color: #008080;"> 4</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 5</span> <span style="color: #000000;">    [Serializable]
</span><span style="color: #008080;"> 6</span>     <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">abstract</span> <span style="color: #0000ff;">class</span><span style="color: #000000;"> FileObject
</span><span style="color: #008080;"> 7</span> <span style="color: #000000;">    {
</span><span style="color: #008080;"> 8</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 9</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 主键.
</span><span style="color: #008080;">10</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> main key.
</span><span style="color: #008080;">11</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">12</span>         <span style="color: #0000ff;">public</span> Guid Id { <span style="color: #0000ff;">get</span>; <span style="color: #0000ff;">set</span><span style="color: #000000;">; }
</span><span style="color: #008080;">13</span> 
<span style="color: #008080;">14</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">15</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 创建一个文件对象，并自动为其生成一个全局唯一的Id。
</span><span style="color: #008080;">16</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">Create a </span><span style="color: #808080;">&lt;see cref="FileObject"/&gt;</span><span style="color: #008000;"> and generate a global unique id for it.</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">17</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">18</span>         <span style="color: #0000ff;">public</span><span style="color: #000000;"> FileObject()
</span><span style="color: #008080;">19</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">20</span>             <span style="color: #0000ff;">this</span>.Id =<span style="color: #000000;"> Guid.NewGuid();
</span><span style="color: #008080;">21</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">22</span> 
<span style="color: #008080;">23</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">string</span><span style="color: #000000;"> ToString()
</span><span style="color: #008080;">24</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">25</span>             <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">string</span>.Format(<span style="color: #800000;">"</span><span style="color: #800000;">Id: {0}</span><span style="color: #800000;">"</span>, <span style="color: #0000ff;">this</span><span style="color: #000000;">.Id);
</span><span style="color: #008080;">26</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">27</span>     }</pre>
</div>
<p><span style="line-height: 1.5;">&nbsp;</span></p>
<h2>数据库(FileDBContext)</h2>
<p>一个数据库上下文负责各种类型的文件对象的CRUD操作。</p>
<div class="cnblogs_code" onclick="cnblogs_code_show('bb877549-849f-4215-994c-dee5a427cd28')"><img id="code_img_closed_bb877549-849f-4215-994c-dee5a427cd28" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_bb877549-849f-4215-994c-dee5a427cd28" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('bb877549-849f-4215-994c-dee5a427cd28',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_bb877549-849f-4215-994c-dee5a427cd28" class="cnblogs_code_hide">
<pre><span style="color: #008080;">  1</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">  2</span> <span style="color: #808080;">///</span><span style="color: #008000;"> 文件数据库。
</span><span style="color: #008080;">  3</span>     <span style="color: #808080;">///</span><span style="color: #008000;"> Represents a file database.
</span><span style="color: #008080;">  4</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">  5</span>     <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">class</span><span style="color: #000000;"> FileDBContext
</span><span style="color: #008080;">  6</span> <span style="color: #000000;">    {
</span><span style="color: #008080;">  7</span>         <span style="color: #0000ff;">#region</span> Fields
<span style="color: #008080;">  8</span> 
<span style="color: #008080;">  9</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 10</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 文件数据库操作锁
</span><span style="color: #008080;"> 11</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">database operation lock.</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;"> 12</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 13</span>         <span style="color: #0000ff;">protected</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">readonly</span> <span style="color: #0000ff;">object</span> operationLock = <span style="color: #0000ff;">new</span> <span style="color: #0000ff;">object</span><span style="color: #000000;">();
</span><span style="color: #008080;"> 14</span> 
<span style="color: #008080;"> 15</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 16</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 文件数据库
</span><span style="color: #008080;"> 17</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">Represents a file database.</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;"> 18</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 19</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="directory"&gt;</span><span style="color: #008000;">数据库文件所在目录</span><span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">Directory for all files of database.</span><span style="color: #808080;">&lt;/para&gt;&lt;/param&gt;</span>
<span style="color: #008080;"> 20</span>         <span style="color: #0000ff;">public</span> FileDBContext(<span style="color: #0000ff;">string</span> directory = <span style="color: #0000ff;">null</span><span style="color: #000000;">)
</span><span style="color: #008080;"> 21</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 22</span>             <span style="color: #0000ff;">if</span> (directory == <span style="color: #0000ff;">null</span><span style="color: #000000;">)
</span><span style="color: #008080;"> 23</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 24</span>                 <span style="color: #0000ff;">this</span>.Directory =<span style="color: #000000;"> Environment.CurrentDirectory;
</span><span style="color: #008080;"> 25</span> <span style="color: #000000;">            }
</span><span style="color: #008080;"> 26</span>             <span style="color: #0000ff;">else</span>
<span style="color: #008080;"> 27</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 28</span>                 Directory =<span style="color: #000000;"> directory;
</span><span style="color: #008080;"> 29</span> <span style="color: #000000;">            }
</span><span style="color: #008080;"> 30</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 31</span> 
<span style="color: #008080;"> 32</span>         <span style="color: #0000ff;">#endregion</span>
<span style="color: #008080;"> 33</span> 
<span style="color: #008080;"> 34</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">string</span><span style="color: #000000;"> ToString()
</span><span style="color: #008080;"> 35</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 36</span>             <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">string</span>.Format(<span style="color: #800000;">"</span><span style="color: #800000;">@: {0}</span><span style="color: #800000;">"</span><span style="color: #000000;">, Directory);
</span><span style="color: #008080;"> 37</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 38</span> 
<span style="color: #008080;"> 39</span>         <span style="color: #0000ff;">#region</span> Properties
<span style="color: #008080;"> 40</span> 
<span style="color: #008080;"> 41</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 42</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 数据库文件所在目录
</span><span style="color: #008080;"> 43</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">Directory of database files.</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;"> 44</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 45</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">virtual</span> <span style="color: #0000ff;">string</span> Directory { <span style="color: #0000ff;">get</span>; <span style="color: #0000ff;">protected</span> <span style="color: #0000ff;">set</span><span style="color: #000000;">; }
</span><span style="color: #008080;"> 46</span> 
<span style="color: #008080;"> 47</span>         <span style="color: #0000ff;">#endregion</span>
<span style="color: #008080;"> 48</span> 
<span style="color: #008080;"> 49</span> 
<span style="color: #008080;"> 50</span>         <span style="color: #0000ff;">protected</span> <span style="color: #0000ff;">string</span><span style="color: #000000;"> Serialize(FileObject item)
</span><span style="color: #008080;"> 51</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 52</span>             <span style="color: #0000ff;">using</span> (StringWriterWithEncoding sw = <span style="color: #0000ff;">new</span><span style="color: #000000;"> StringWriterWithEncoding(Encoding.UTF8))
</span><span style="color: #008080;"> 53</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 54</span>                 XmlSerializer serializer = <span style="color: #0000ff;">new</span><span style="color: #000000;"> XmlSerializer(item.GetType());
</span><span style="color: #008080;"> 55</span> <span style="color: #000000;">                serializer.Serialize(sw, item);
</span><span style="color: #008080;"> 56</span>                 <span style="color: #0000ff;">string</span> serializedString =<span style="color: #000000;"> sw.ToString();
</span><span style="color: #008080;"> 57</span> 
<span style="color: #008080;"> 58</span>                 <span style="color: #0000ff;">return</span><span style="color: #000000;"> serializedString;
</span><span style="color: #008080;"> 59</span> <span style="color: #000000;">            }
</span><span style="color: #008080;"> 60</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 61</span> 
<span style="color: #008080;"> 62</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 63</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 将字符串反序列化成文档对象
</span><span style="color: #008080;"> 64</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 65</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;typeparam name="TDocument"&gt;</span><span style="color: #008000;">文档类型</span><span style="color: #808080;">&lt;/typeparam&gt;</span>
<span style="color: #008080;"> 66</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="serializedFileObject"&gt;</span><span style="color: #008000;">字符串</span><span style="color: #808080;">&lt;/param&gt;</span>
<span style="color: #008080;"> 67</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;</span>
<span style="color: #008080;"> 68</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 文档对象
</span><span style="color: #008080;"> 69</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/returns&gt;</span>
<span style="color: #008080;"> 70</span>         <span style="color: #0000ff;">protected</span> TFileObject Deserialize&lt;TFileObject&gt;(<span style="color: #0000ff;">string</span><span style="color: #000000;"> serializedFileObject)
</span><span style="color: #008080;"> 71</span>             <span style="color: #0000ff;">where</span><span style="color: #000000;"> TFileObject : FileObject
</span><span style="color: #008080;"> 72</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 73</span>             <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">string</span><span style="color: #000000;">.IsNullOrEmpty(serializedFileObject))
</span><span style="color: #008080;"> 74</span>                 <span style="color: #0000ff;">throw</span> <span style="color: #0000ff;">new</span> ArgumentNullException(<span style="color: #800000;">"</span><span style="color: #800000;">data</span><span style="color: #800000;">"</span><span style="color: #000000;">);
</span><span style="color: #008080;"> 75</span> 
<span style="color: #008080;"> 76</span>             <span style="color: #0000ff;">using</span> (StringReader sr = <span style="color: #0000ff;">new</span><span style="color: #000000;"> StringReader(serializedFileObject))
</span><span style="color: #008080;"> 77</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 78</span>                 XmlSerializer serializer = <span style="color: #0000ff;">new</span> XmlSerializer(<span style="color: #0000ff;">typeof</span><span style="color: #000000;">(TFileObject));
</span><span style="color: #008080;"> 79</span>                 <span style="color: #0000ff;">object</span> deserializedObj =<span style="color: #000000;"> serializer.Deserialize(sr);
</span><span style="color: #008080;"> 80</span>                 TFileObject fileObject = deserializedObj <span style="color: #0000ff;">as</span><span style="color: #000000;"> TFileObject;
</span><span style="color: #008080;"> 81</span>                 <span style="color: #0000ff;">return</span><span style="color: #000000;"> fileObject;
</span><span style="color: #008080;"> 82</span> <span style="color: #000000;">            }
</span><span style="color: #008080;"> 83</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 84</span> 
<span style="color: #008080;"> 85</span>         <span style="color: #0000ff;">protected</span> <span style="color: #0000ff;">string</span><span style="color: #000000;"> GenerateFileFullPath(FileObject item)
</span><span style="color: #008080;"> 86</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 87</span>             <span style="color: #0000ff;">string</span> path =<span style="color: #000000;"> GenerateFilePath(item.GetType());
</span><span style="color: #008080;"> 88</span>             <span style="color: #0000ff;">string</span> name =<span style="color: #000000;"> item.GenerateFileName();
</span><span style="color: #008080;"> 89</span>             <span style="color: #0000ff;">string</span> fullname =<span style="color: #000000;"> Path.Combine(path, name);
</span><span style="color: #008080;"> 90</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> fullname;
</span><span style="color: #008080;"> 91</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 92</span> 
<span style="color: #008080;"> 93</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 94</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 生成文件路径
</span><span style="color: #008080;"> 95</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 96</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;typeparam name="TDocument"&gt;</span><span style="color: #008000;">文档类型</span><span style="color: #808080;">&lt;/typeparam&gt;</span>
<span style="color: #008080;"> 97</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;</span><span style="color: #008000;">文件路径</span><span style="color: #808080;">&lt;/returns&gt;</span>
<span style="color: #008080;"> 98</span>         <span style="color: #0000ff;">protected</span> <span style="color: #0000ff;">string</span><span style="color: #000000;"> GenerateFilePath(Type type)
</span><span style="color: #008080;"> 99</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">100</span>             <span style="color: #0000ff;">string</span> path = Path.Combine(<span style="color: #0000ff;">this</span><span style="color: #000000;">.Directory, type.Name);
</span><span style="color: #008080;">101</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> path;
</span><span style="color: #008080;">102</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">103</span> 
<span style="color: #008080;">104</span>         <span style="color: #0000ff;">#region</span> CRUD
<span style="color: #008080;">105</span> 
<span style="color: #008080;">106</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">107</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 增加一个</span><span style="color: #808080;">&lt;see cref="FileObject"/&gt;</span><span style="color: #008000;">到数据库。这实际上创建了一个文件。
</span><span style="color: #008080;">108</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">Create a new </span><span style="color: #808080;">&lt;see cref="FileObject"/&gt;</span><span style="color: #008000;"> into database. This operation will create a new file.</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">109</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">110</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="item"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">111</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">virtual</span> <span style="color: #0000ff;">void</span><span style="color: #000000;"> Create(FileObject item)
</span><span style="color: #008080;">112</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">113</span>             <span style="color: #0000ff;">string</span> fileName =<span style="color: #000000;"> GenerateFileFullPath(item);
</span><span style="color: #008080;">114</span>             <span style="color: #0000ff;">string</span> output =<span style="color: #000000;"> Serialize(item);
</span><span style="color: #008080;">115</span> 
<span style="color: #008080;">116</span>             <span style="color: #0000ff;">lock</span><span style="color: #000000;"> (operationLock)
</span><span style="color: #008080;">117</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">118</span>                 System.IO.FileInfo info = <span style="color: #0000ff;">new</span><span style="color: #000000;"> System.IO.FileInfo(fileName);
</span><span style="color: #008080;">119</span> <span style="color: #000000;">                System.IO.Directory.CreateDirectory(info.Directory.FullName);
</span><span style="color: #008080;">120</span> <span style="color: #000000;">                System.IO.File.WriteAllText(fileName, output);
</span><span style="color: #008080;">121</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">122</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">123</span> 
<span style="color: #008080;">124</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">125</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 检索符合给定条件的所有</span><span style="color: #808080;">&lt;paramref name="TFileObject"/&gt;</span><span style="color: #008000;">。
</span><span style="color: #008080;">126</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">Retrives all </span><span style="color: #808080;">&lt;paramref name="TFileObject"/&gt;</span><span style="color: #008000;"> that satisfies the specified condition.</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">127</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">128</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;typeparam name="TFileObject"&gt;&lt;/typeparam&gt;</span>
<span style="color: #008080;">129</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="predicate"&gt;</span><span style="color: #008000;">检索出的对象应满足的条件。</span><span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">THe condition that should be satisfied by retrived object.</span><span style="color: #808080;">&lt;/para&gt;&lt;/param&gt;</span>
<span style="color: #008080;">130</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;">131</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">virtual</span> IList&lt;TFileObject&gt; Retrieve&lt;TFileObject&gt;(Predicate&lt;TFileObject&gt;<span style="color: #000000;"> predicate)
</span><span style="color: #008080;">132</span>             <span style="color: #0000ff;">where</span><span style="color: #000000;"> TFileObject : FileObject
</span><span style="color: #008080;">133</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">134</span>             IList&lt;TFileObject&gt; result = <span style="color: #0000ff;">new</span> List&lt;TFileObject&gt;<span style="color: #000000;">();
</span><span style="color: #008080;">135</span>             <span style="color: #0000ff;">if</span> (predicate != <span style="color: #0000ff;">null</span><span style="color: #000000;">)
</span><span style="color: #008080;">136</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">137</span>                 <span style="color: #0000ff;">string</span> path = GenerateFilePath(<span style="color: #0000ff;">typeof</span><span style="color: #000000;">(TFileObject));
</span><span style="color: #008080;">138</span>                 <span style="color: #0000ff;">string</span>[] files = System.IO.Directory.GetFiles(path, <span style="color: #800000;">"</span><span style="color: #800000;">*.xml</span><span style="color: #800000;">"</span><span style="color: #000000;">, SearchOption.AllDirectories);
</span><span style="color: #008080;">139</span>                 <span style="color: #0000ff;">foreach</span> (<span style="color: #0000ff;">var</span> item <span style="color: #0000ff;">in</span><span style="color: #000000;"> files)
</span><span style="color: #008080;">140</span> <span style="color: #000000;">                {
</span><span style="color: #008080;">141</span>                     <span style="color: #0000ff;">string</span> fileContent =<span style="color: #000000;"> File.ReadAllText(item);
</span><span style="color: #008080;">142</span>                     TFileObject deserializedFileObject = Deserialize&lt;TFileObject&gt;<span style="color: #000000;">(fileContent);
</span><span style="color: #008080;">143</span>                     <span style="color: #0000ff;">if</span><span style="color: #000000;"> (predicate(deserializedFileObject))
</span><span style="color: #008080;">144</span> <span style="color: #000000;">                    {
</span><span style="color: #008080;">145</span> <span style="color: #000000;">                        result.Add(deserializedFileObject);
</span><span style="color: #008080;">146</span> <span style="color: #000000;">                    }
</span><span style="color: #008080;">147</span> <span style="color: #000000;">                }
</span><span style="color: #008080;">148</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">149</span> 
<span style="color: #008080;">150</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> result;
</span><span style="color: #008080;">151</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">152</span> 
<span style="color: #008080;">153</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">154</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 更新给定的对象。
</span><span style="color: #008080;">155</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">Update specified </span><span style="color: #808080;">&lt;paramref name="FileObject"/&gt;</span><span style="color: #008000;">.</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">156</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">157</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="item"&gt;</span><span style="color: #008000;">要被更新的对象。</span><span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">The object to be updated.</span><span style="color: #808080;">&lt;/para&gt;&lt;/param&gt;</span>
<span style="color: #008080;">158</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">virtual</span> <span style="color: #0000ff;">void</span><span style="color: #000000;"> Update(FileObject item)
</span><span style="color: #008080;">159</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">160</span>             <span style="color: #0000ff;">string</span> fileName =<span style="color: #000000;"> GenerateFileFullPath(item);
</span><span style="color: #008080;">161</span>             <span style="color: #0000ff;">string</span> output =<span style="color: #000000;"> Serialize(item);
</span><span style="color: #008080;">162</span> 
<span style="color: #008080;">163</span>             <span style="color: #0000ff;">lock</span><span style="color: #000000;"> (operationLock)
</span><span style="color: #008080;">164</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">165</span>                 System.IO.FileInfo info = <span style="color: #0000ff;">new</span><span style="color: #000000;"> System.IO.FileInfo(fileName);
</span><span style="color: #008080;">166</span> <span style="color: #000000;">                System.IO.Directory.CreateDirectory(info.Directory.FullName);
</span><span style="color: #008080;">167</span> <span style="color: #000000;">                System.IO.File.WriteAllText(fileName, output);
</span><span style="color: #008080;">168</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">169</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">170</span> 
<span style="color: #008080;">171</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">172</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 删除指定的对象。
</span><span style="color: #008080;">173</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">Delete specified </span><span style="color: #808080;">&lt;paramref name="FileObject"/&gt;</span><span style="color: #008000;">.</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">174</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">175</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="item"&gt;</span><span style="color: #008000;">要被删除的对象。</span><span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">The object to be deleted.</span><span style="color: #808080;">&lt;/para&gt;&lt;/param&gt;</span>
<span style="color: #008080;">176</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">virtual</span> <span style="color: #0000ff;">void</span><span style="color: #000000;"> Delete(FileObject item)
</span><span style="color: #008080;">177</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">178</span>             <span style="color: #0000ff;">if</span> (item == <span style="color: #0000ff;">null</span><span style="color: #000000;">)
</span><span style="color: #008080;">179</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">180</span>                 <span style="color: #0000ff;">throw</span> <span style="color: #0000ff;">new</span><span style="color: #000000;"> ArgumentNullException(item.ToString());
</span><span style="color: #008080;">181</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">182</span> 
<span style="color: #008080;">183</span>             <span style="color: #0000ff;">string</span> filename =<span style="color: #000000;"> GenerateFileFullPath(item);
</span><span style="color: #008080;">184</span>             <span style="color: #0000ff;">if</span><span style="color: #000000;"> (File.Exists(filename))
</span><span style="color: #008080;">185</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">186</span>                 <span style="color: #0000ff;">lock</span><span style="color: #000000;"> (operationLock)
</span><span style="color: #008080;">187</span> <span style="color: #000000;">                {
</span><span style="color: #008080;">188</span> <span style="color: #000000;">                    File.Delete(filename);
</span><span style="color: #008080;">189</span> <span style="color: #000000;">                }
</span><span style="color: #008080;">190</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">191</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">192</span> 
<span style="color: #008080;">193</span>         <span style="color: #0000ff;">#endregion</span> CRUD
<span style="color: #008080;">194</span> 
<span style="color: #008080;">195</span>     }</pre>
</div>
<span class="cnblogs_code_collapse">FileDBContext</span></div>
<p><span style="line-height: 1.5;">&nbsp;</span></p>
<h2>文件存储方式(Way to store files)</h2>
<p>在数据库目录下，SharpFileDB为每个表类型创建一个文件夹，在各自文件夹内存储每个对象。每个对象都占用一个XML文件。暂时用XML格式，因为是.NET内置的格式，省的再找外部序列化工具。XML文件名与其对应的对象Id相同。</p>
<p><img src="http://images0.cnblogs.com/blog2015/383191/201506/220150025954911.png" alt="" /></p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<h1>下载(Download)</h1>
<p>我已将源码放到（<a href="https://github.com/bitzhuwei/SharpFileDB/" target="_blank">https://github.com/bitzhuwei/SharpFileDB/</a>），欢迎试用、提建议或Fork此项目。</p>
<h1>更新(Update)</h1>
<p>2015-06-22</p>
<p>增加了序列化接口(IPersistence)，使得FileDBContext可以选择序列化器。</p>
<p>增加了二进制序列化类型(BinaryPersistence)。</p>
<p>使用Convert.ToBase64String()和Convert.FromBase64String()实现Byte数组与string之间的转换。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;">1</span> <span style="color: #008000;">//</span><span style="color: #008000;">Image--&gt;Byte[]--&gt;String </span>
<span style="color: #008080;">2</span>  Byte[] bytes = File.ReadAllBytes(<span style="color: #800000;">@"</span><span style="color: #800000;">d:\a.gif</span><span style="color: #800000;">"</span><span style="color: #000000;">); 
</span><span style="color: #008080;">3</span>  MemoryStream ms = <span style="color: #0000ff;">new</span><span style="color: #000000;"> MemoryStream(bty); 
</span><span style="color: #008080;">4</span>  String imgStr =<span style="color: #000000;"> Convert.ToBase64String(ms.ToArray());
</span><span style="color: #008080;">5</span> 
<span style="color: #008080;">6</span> <span style="color: #008000;">//</span><span style="color: #008000;">String--&gt;Byte[]--&gt;Image </span>
<span style="color: #008080;">7</span>  <span style="color: #0000ff;">byte</span>[] imgBytes =<span style="color: #000000;"> Convert.FromBase64String(imgStr); 
</span><span style="color: #008080;">8</span>  Response.BinaryWrite(imgBytes.ToArray());  <span style="color: #008000;">//</span><span style="color: #008000;"> 将一个二制字符串写入HTTP输出流</span></pre>
</div>
<p>&nbsp;</p>
<p>修改了接口IPersistence，让它直接进行内存数据与文件之间的转化。这样，即使序列化的结果是byte[]或其它类型，也可以直接保存到文件，不再需要先转化为string后再保存。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 2</span>     <span style="color: #808080;">///</span><span style="color: #008000;"> 文件数据库使用此接口进行持久化相关的操作。
</span><span style="color: #008080;"> 3</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">File database executes persistence operations via this interface.</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;"> 4</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 5</span>     <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">interface</span><span style="color: #000000;"> IPersistence
</span><span style="color: #008080;"> 6</span> <span style="color: #000000;">    {
</span><span style="color: #008080;"> 7</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 8</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;see cref="FileObject"/&gt;</span><span style="color: #008000;">文件的扩展名。
</span><span style="color: #008080;"> 9</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> Extension name of </span><span style="color: #808080;">&lt;see cref="FileObject"/&gt;</span><span style="color: #008000;">'s file.
</span><span style="color: #008080;">10</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">11</span>         <span style="color: #0000ff;">string</span> Extension { <span style="color: #0000ff;">get</span><span style="color: #000000;">; }
</span><span style="color: #008080;">12</span> 
<span style="color: #008080;">13</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">14</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 将文件对象序列化为文件。
</span><span style="color: #008080;">15</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">Serialize the specified </span><span style="color: #808080;">&lt;paramref name="item"/&gt;</span><span style="color: #008000;"> into </span><span style="color: #808080;">&lt;paramref name="fullname"/&gt;</span><span style="color: #008000;">.</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">16</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">17</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="item"&gt;</span><span style="color: #008000;">要进行序列化的文件对象。</span><span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">file object to be serialized.</span><span style="color: #808080;">&lt;/para&gt;&lt;/param&gt;</span>
<span style="color: #008080;">18</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="fullname"&gt;</span><span style="color: #008000;">要保存到的文件的绝对路径。</span><span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">file's fullname.</span><span style="color: #808080;">&lt;/para&gt;&lt;/param&gt;</span>
<span style="color: #008080;">19</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;">20</span>         <span style="color: #0000ff;">void</span> Serialize([Required] FileObject item, [Required] <span style="color: #0000ff;">string</span><span style="color: #000000;"> fullname);
</span><span style="color: #008080;">21</span> 
<span style="color: #008080;">22</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">23</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 将文件反序列化成文件对象。
</span><span style="color: #008080;">24</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">Deserialize the specified file to an instance of </span><span style="color: #808080;">&lt;paramref name="TFileObject"/&gt;</span><span style="color: #008000;">.</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">25</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">26</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;typeparam name="TFileObject"&gt;&lt;/typeparam&gt;</span>
<span style="color: #008080;">27</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="serializedFileObject"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">28</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;">29</span>         TFileObject Deserialize&lt;TFileObject&gt;([Required] <span style="color: #0000ff;">string</span> fullname) <span style="color: #0000ff;">where</span><span style="color: #000000;"> TFileObject : FileObject;
</span><span style="color: #008080;">30</span>     }</pre>
</div>
<p>&nbsp;</p>
<p>使用接口ISerializable，让每个FileObject都自行处理自己的字段、属性的序列化和反序列化动作（保存、忽略等）。</p>
<div class="cnblogs_code" onclick="cnblogs_code_show('ed552828-3309-4df8-b80c-41df7bf72fbc')"><img id="code_img_closed_ed552828-3309-4df8-b80c-41df7bf72fbc" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_ed552828-3309-4df8-b80c-41df7bf72fbc" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('ed552828-3309-4df8-b80c-41df7bf72fbc',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_ed552828-3309-4df8-b80c-41df7bf72fbc" class="cnblogs_code_hide">
<pre><span style="color: #008080;"> 1</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 2</span>     <span style="color: #808080;">///</span><span style="color: #008000;"> 可在文件数据库中使用CRUD操作的所有类型的基类。类似于关系数据库中的Table。
</span><span style="color: #008080;"> 3</span>     <span style="color: #808080;">///</span><span style="color: #008000;"> Base class for all classed that can use CRUD in SharpFileDB. It's similar to the concept 'table' in relational database.
</span><span style="color: #008080;"> 4</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 5</span> <span style="color: #000000;">    [Serializable]
</span><span style="color: #008080;"> 6</span>     <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">abstract</span> <span style="color: #0000ff;">class</span><span style="color: #000000;"> FileObject : ISerializable
</span><span style="color: #008080;"> 7</span> <span style="color: #000000;">    {
</span><span style="color: #008080;"> 8</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 9</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 用以区分每个Table的每条记录。
</span><span style="color: #008080;">10</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> This Id is used for diffrentiate instances of 'table's.
</span><span style="color: #008080;">11</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">12</span>         <span style="color: #0000ff;">public</span> Guid Id { <span style="color: #0000ff;">get</span>; <span style="color: #0000ff;">internal</span> <span style="color: #0000ff;">set</span><span style="color: #000000;">; }
</span><span style="color: #008080;">13</span> 
<span style="color: #008080;">14</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">15</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 创建一个文件对象，在用</span><span style="color: #808080;">&lt;code&gt;</span><span style="color: #008000;">FileDBContext.Create();</span><span style="color: #808080;">&lt;/code&gt;</span><span style="color: #008000;">将此对象保存到数据库之前，此对象的Id为</span><span style="color: #808080;">&lt;code&gt;</span><span style="color: #008000;">Guid.Empty</span><span style="color: #808080;">&lt;/code&gt;</span><span style="color: #008000;">。
</span><span style="color: #008080;">16</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">Create a </span><span style="color: #808080;">&lt;see cref="FileObject"/&gt;</span><span style="color: #008000;"> whose Id is </span><span style="color: #808080;">&lt;code&gt;</span><span style="color: #008000;">Guid.Empty</span><span style="color: #808080;">&lt;/code&gt;</span><span style="color: #008000;"> until it's saved to database by </span><span style="color: #808080;">&lt;code&gt;</span><span style="color: #008000;">FileDBContext.Create();</span><span style="color: #808080;">&lt;/code&gt;</span><span style="color: #008000;">.</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">17</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">18</span>         <span style="color: #0000ff;">public</span><span style="color: #000000;"> FileObject()
</span><span style="color: #008080;">19</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">20</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">21</span> 
<span style="color: #008080;">22</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">23</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 生成文件名，此文件将用于存储此</span><span style="color: #808080;">&lt;see cref="FileObject"/&gt;</span><span style="color: #008000;">的内容。
</span><span style="color: #008080;">24</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> Generate file name that will contain this instance's data of </span><span style="color: #808080;">&lt;see cref="FileObject"/&gt;</span><span style="color: #008000;">.
</span><span style="color: #008080;">25</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">26</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="extension"&gt;</span><span style="color: #008000;">文件扩展名。</span><span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">File's extension name.</span><span style="color: #808080;">&lt;/para&gt;&lt;/param&gt;</span>
<span style="color: #008080;">27</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;">28</span>         <span style="color: #0000ff;">internal</span> <span style="color: #0000ff;">string</span> GenerateFileName([Required] <span style="color: #0000ff;">string</span><span style="color: #000000;"> extension)
</span><span style="color: #008080;">29</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">30</span>             <span style="color: #0000ff;">string</span> id = <span style="color: #0000ff;">this</span><span style="color: #000000;">.Id.ToString();
</span><span style="color: #008080;">31</span> 
<span style="color: #008080;">32</span>             <span style="color: #0000ff;">string</span> name = <span style="color: #0000ff;">string</span>.Format(CultureInfo.InvariantCulture, <span style="color: #800000;">"</span><span style="color: #800000;">{0}.{1}</span><span style="color: #800000;">"</span><span style="color: #000000;">, id, extension);
</span><span style="color: #008080;">33</span> 
<span style="color: #008080;">34</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> name;
</span><span style="color: #008080;">35</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">36</span> 
<span style="color: #008080;">37</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">string</span><span style="color: #000000;"> ToString()
</span><span style="color: #008080;">38</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">39</span>             <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">string</span>.Format(<span style="color: #800000;">"</span><span style="color: #800000;">Id: {0}</span><span style="color: #800000;">"</span>, <span style="color: #0000ff;">this</span><span style="color: #000000;">.Id);
</span><span style="color: #008080;">40</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">41</span> 
<span style="color: #008080;">42</span>         <span style="color: #0000ff;">const</span> <span style="color: #0000ff;">string</span> strGuid = <span style="color: #800000;">"</span><span style="color: #800000;">Guid</span><span style="color: #800000;">"</span><span style="color: #000000;">;
</span><span style="color: #008080;">43</span> 
<span style="color: #008080;">44</span>         <span style="color: #0000ff;">#region</span> ISerializable 成员
<span style="color: #008080;">45</span> 
<span style="color: #008080;">46</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">47</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> This method will be invoked automatically when IFormatter.Serialize() is called.
</span><span style="color: #008080;">48</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">You must use </span><span style="color: #808080;">&lt;code&gt;</span><span style="color: #008000;">base(info, context);</span><span style="color: #808080;">&lt;/code&gt;</span><span style="color: #008000;"> in the derived class to feed </span><span style="color: #808080;">&lt;see cref="FileObject"/&gt;</span><span style="color: #008000;">'s fields and properties.</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">49</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">当使用IFormatter.Serialize()时会自动调用此方法。</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">50</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">继承此类型时，必须在子类型中用</span><span style="color: #808080;">&lt;code&gt;</span><span style="color: #008000;">base(info, context);</span><span style="color: #808080;">&lt;/code&gt;</span><span style="color: #008000;">来填充</span><span style="color: #808080;">&lt;see cref="FileObject"/&gt;</span><span style="color: #008000;">自身的数据。</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">51</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">52</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="info"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">53</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="context"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">54</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">virtual</span> <span style="color: #0000ff;">void</span><span style="color: #000000;"> GetObjectData([Required] SerializationInfo info, StreamingContext context)
</span><span style="color: #008080;">55</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">56</span>             info.AddValue(strGuid, <span style="color: #0000ff;">this</span><span style="color: #000000;">.Id.ToString());
</span><span style="color: #008080;">57</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">58</span> 
<span style="color: #008080;">59</span>         <span style="color: #0000ff;">#endregion</span>
<span style="color: #008080;">60</span> 
<span style="color: #008080;">61</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">62</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> This method will be invoked automatically when IFormatter.Serialize() is called.
</span><span style="color: #008080;">63</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">You must use </span><span style="color: #808080;">&lt;code&gt;</span><span style="color: #008000;">: base(info, context)</span><span style="color: #808080;">&lt;/code&gt;</span><span style="color: #008000;"> in the derived class to feed </span><span style="color: #808080;">&lt;see cref="FileObject"/&gt;</span><span style="color: #008000;">'s fields and properties.</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">64</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">当使用IFormatter.Serialize()时会自动调用此方法。</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">65</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">继承此类型时，必须在子类型中用</span><span style="color: #808080;">&lt;code&gt;</span><span style="color: #008000;">: base(info, context)</span><span style="color: #808080;">&lt;/code&gt;</span><span style="color: #008000;">来填充</span><span style="color: #808080;">&lt;see cref="FileObject"/&gt;</span><span style="color: #008000;">自身的数据。</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;">66</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">67</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="info"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">68</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="context"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">69</span>         <span style="color: #0000ff;">protected</span><span style="color: #000000;"> FileObject([Required] SerializationInfo info, StreamingContext context)
</span><span style="color: #008080;">70</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">71</span>             <span style="color: #0000ff;">string</span> str = (<span style="color: #0000ff;">string</span>)info.GetValue(strGuid, <span style="color: #0000ff;">typeof</span>(<span style="color: #0000ff;">string</span><span style="color: #000000;">));
</span><span style="color: #008080;">72</span>             <span style="color: #0000ff;">this</span>.Id =<span style="color: #000000;"> Guid.Parse(str);
</span><span style="color: #008080;">73</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">74</span>     }</pre>
</div>
<span class="cnblogs_code_collapse">FileObject相当于关系数据库中的Table</span></div>
<p>另外，FileObject在使用new FileObject();创建时不为其指定Guid，而在FileDBContext.Create(FileObject)时才进行指定。这样，在反序列化时就不必浪费时间去白白指定一个即将被替换的Guid了。这也更合乎情理：只有那些已经存储到数据库或立刻就要存储到数据库的FileObject才有必要拥有一个Guid。</p>
<p>&nbsp;</p>
<p>用一个DefaultPersistence类型代替了BinaryPersistence和XmlPersistence。由于SoapFormatter和BinaryFormatter是近亲，而XmlSerializer跟他们是远亲；同时SoapFormatter和BinaryFormatter分别实现了文本文件序列化和二进制序列化，XmlSerializer就更不用出场了。因此现在不再使用XmlSerializer。</p>
<div class="cnblogs_code" onclick="cnblogs_code_show('92bbdfc2-c7b0-434d-adb0-f7f5f094f2f7')"><img id="code_img_closed_92bbdfc2-c7b0-434d-adb0-f7f5f094f2f7" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_92bbdfc2-c7b0-434d-adb0-f7f5f094f2f7" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('92bbdfc2-c7b0-434d-adb0-f7f5f094f2f7',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_92bbdfc2-c7b0-434d-adb0-f7f5f094f2f7" class="cnblogs_code_hide">
<pre><span style="color: #008080;"> 1</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 2</span>     <span style="color: #808080;">///</span><span style="color: #008000;"> 用</span><span style="color: #808080;">&lt;see cref="IFormatter"/&gt;</span><span style="color: #008000;">实现</span><span style="color: #808080;">&lt;see cref="IPersistence"/&gt;</span><span style="color: #008000;">。
</span><span style="color: #008080;"> 3</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;para&gt;</span><span style="color: #008000;">Implement </span><span style="color: #808080;">&lt;see cref="IPersistence"/&gt;</span><span style="color: #008000;"> using </span><span style="color: #808080;">&lt;see cref="IFormatter"/&gt;</span><span style="color: #008000;">.</span><span style="color: #808080;">&lt;/para&gt;</span>
<span style="color: #008080;"> 4</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 5</span>     <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">class</span><span style="color: #000000;"> DefaultPersistence : IPersistence
</span><span style="color: #008080;"> 6</span> <span style="color: #000000;">    {
</span><span style="color: #008080;"> 7</span>         <span style="color: #0000ff;">private</span><span style="color: #000000;"> System.Runtime.Serialization.IFormatter formatter;
</span><span style="color: #008080;"> 8</span> 
<span style="color: #008080;"> 9</span>         <span style="color: #0000ff;">public</span> DefaultPersistence(PersistenceFormat format =<span style="color: #000000;"> PersistenceFormat.Soap)
</span><span style="color: #008080;">10</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">11</span>             <span style="color: #0000ff;">switch</span><span style="color: #000000;"> (format)
</span><span style="color: #008080;">12</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">13</span>                 <span style="color: #0000ff;">case</span><span style="color: #000000;"> PersistenceFormat.Soap:
</span><span style="color: #008080;">14</span>                     <span style="color: #0000ff;">this</span>.formatter = <span style="color: #0000ff;">new</span><span style="color: #000000;"> System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
</span><span style="color: #008080;">15</span>                     <span style="color: #0000ff;">this</span>.Extension = <span style="color: #800000;">"</span><span style="color: #800000;">soap</span><span style="color: #800000;">"</span><span style="color: #000000;">;
</span><span style="color: #008080;">16</span>                     <span style="color: #0000ff;">break</span><span style="color: #000000;">;
</span><span style="color: #008080;">17</span>                 <span style="color: #0000ff;">case</span><span style="color: #000000;"> PersistenceFormat.Binary:
</span><span style="color: #008080;">18</span>                     <span style="color: #0000ff;">this</span>.formatter = <span style="color: #0000ff;">new</span><span style="color: #000000;"> System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
</span><span style="color: #008080;">19</span>                     <span style="color: #0000ff;">this</span>.Extension = <span style="color: #800000;">"</span><span style="color: #800000;">bin</span><span style="color: #800000;">"</span><span style="color: #000000;">;
</span><span style="color: #008080;">20</span>                     <span style="color: #0000ff;">break</span><span style="color: #000000;">;
</span><span style="color: #008080;">21</span>                 <span style="color: #0000ff;">default</span><span style="color: #000000;">:
</span><span style="color: #008080;">22</span>                     <span style="color: #0000ff;">throw</span> <span style="color: #0000ff;">new</span><span style="color: #000000;"> NotImplementedException();
</span><span style="color: #008080;">23</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">24</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">25</span> 
<span style="color: #008080;">26</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">enum</span><span style="color: #000000;"> PersistenceFormat
</span><span style="color: #008080;">27</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">28</span> <span style="color: #000000;">            Soap,
</span><span style="color: #008080;">29</span> <span style="color: #000000;">            Binary,
</span><span style="color: #008080;">30</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">31</span> 
<span style="color: #008080;">32</span>         <span style="color: #0000ff;">#region</span> IPersistence 成员
<span style="color: #008080;">33</span> 
<span style="color: #008080;">34</span>         <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">string</span><span style="color: #000000;"> extension;
</span><span style="color: #008080;">35</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">string</span><span style="color: #000000;"> Extension
</span><span style="color: #008080;">36</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">37</span>             <span style="color: #0000ff;">get</span> { <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">this</span><span style="color: #000000;">.extension; }
</span><span style="color: #008080;">38</span>             <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">set</span> { <span style="color: #0000ff;">this</span>.extension =<span style="color: #000000;"> value; }
</span><span style="color: #008080;">39</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">40</span> 
<span style="color: #008080;">41</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">void</span> Serialize(FileObject item, <span style="color: #0000ff;">string</span><span style="color: #000000;"> fullname)
</span><span style="color: #008080;">42</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">43</span>             <span style="color: #0000ff;">if</span> (item == <span style="color: #0000ff;">null</span><span style="color: #000000;">)
</span><span style="color: #008080;">44</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">45</span>                 <span style="color: #0000ff;">throw</span> <span style="color: #0000ff;">new</span> ArgumentNullException(<span style="color: #800000;">"</span><span style="color: #800000;">item</span><span style="color: #800000;">"</span><span style="color: #000000;">);
</span><span style="color: #008080;">46</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">47</span> 
<span style="color: #008080;">48</span>             <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">string</span><span style="color: #000000;">.IsNullOrEmpty(fullname))
</span><span style="color: #008080;">49</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">50</span>                 <span style="color: #0000ff;">throw</span> <span style="color: #0000ff;">new</span> ArgumentNullException(<span style="color: #800000;">"</span><span style="color: #800000;">fullname</span><span style="color: #800000;">"</span><span style="color: #000000;">);
</span><span style="color: #008080;">51</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">52</span> 
<span style="color: #008080;">53</span>             <span style="color: #0000ff;">using</span> (FileStream s = <span style="color: #0000ff;">new</span><span style="color: #000000;"> FileStream(fullname, FileMode.Create, FileAccess.Write))
</span><span style="color: #008080;">54</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">55</span> <span style="color: #000000;">                formatter.Serialize(s, item);
</span><span style="color: #008080;">56</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">57</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">58</span> 
<span style="color: #008080;">59</span>         <span style="color: #0000ff;">public</span> TFileObject Deserialize&lt;TFileObject&gt;(<span style="color: #0000ff;">string</span> fullname) <span style="color: #0000ff;">where</span><span style="color: #000000;"> TFileObject : FileObject
</span><span style="color: #008080;">60</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">61</span>             <span style="color: #0000ff;">if</span>(<span style="color: #0000ff;">string</span><span style="color: #000000;">.IsNullOrEmpty(fullname))
</span><span style="color: #008080;">62</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">63</span>                 <span style="color: #0000ff;">throw</span> <span style="color: #0000ff;">new</span> ArgumentNullException(<span style="color: #800000;">"</span><span style="color: #800000;">fullname</span><span style="color: #800000;">"</span><span style="color: #000000;">);
</span><span style="color: #008080;">64</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">65</span> 
<span style="color: #008080;">66</span>             TFileObject fileObject = <span style="color: #0000ff;">null</span><span style="color: #000000;">;
</span><span style="color: #008080;">67</span> 
<span style="color: #008080;">68</span>             <span style="color: #0000ff;">using</span> (FileStream s = <span style="color: #0000ff;">new</span><span style="color: #000000;"> FileStream(fullname, FileMode.Open, FileAccess.Read))
</span><span style="color: #008080;">69</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">70</span>                 <span style="color: #0000ff;">object</span> obj =<span style="color: #000000;"> formatter.Deserialize(s);
</span><span style="color: #008080;">71</span>                 fileObject = obj <span style="color: #0000ff;">as</span><span style="color: #000000;"> TFileObject;
</span><span style="color: #008080;">72</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">73</span> 
<span style="color: #008080;">74</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> fileObject;
</span><span style="color: #008080;">75</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">76</span> 
<span style="color: #008080;">77</span>         <span style="color: #0000ff;">#endregion</span>
<span style="color: #008080;">78</span> 
<span style="color: #008080;">79</span>     }</pre>
</div>
<span class="cnblogs_code_collapse">支持Soap和binary的持久化工具。</span></div>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>PS：我国大多数县的人口为几万到几十万。目前，县里各种政府部门急需实现信息化网络化办公办事，但他们一般用不起那种月薪上万的开发者和高端软件公司。我注意到，一个县级政府部门日常应对的人群数量就是<span style="color: #ff0000;">万人</span>左右，甚至常常是千人左右。所以他们不需要太高端复杂的系统设计，用支持万人级别的数据库就可以了。另一方面，初级开发者也不能充分利用那些看似高端复杂的数据库的优势。做个小型系统而已，还是简单一点好。</p>
<p>所以我就想做这样一个小型文件数据库，我相信这会帮助很多人。能以己所学惠及大众，才是我们的价值所在。.</p>
