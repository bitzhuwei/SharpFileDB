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
<h2>数据库(FileDBContext)</h2>
<p>一个数据库上下文负责各种类型的文件对象的CRUD操作。</p>
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
<p>&nbsp;</p>
<p>修改了接口IPersistence，让它直接进行内存数据与文件之间的转化。这样，即使序列化的结果是byte[]或其它类型，也可以直接保存到文件，不再需要先转化为string后再保存。</p>
<p>&nbsp;</p>
<p>使用接口ISerializable，让每个FileObject都自行处理自己的字段、属性的序列化和反序列化动作（保存、忽略等）。</p>
<p><span style="font-family: verdana, Arial, Helvetica, sans-serif; font-size: 14px; line-height: 1.5;">另外，FileObject在使用new FileObject();创建时不为其指定Guid，而在FileDBContext.Create(FileObject)时才进行指定。这样，在反序列化时就不必浪费时间去白白指定一个即将被替换的Guid了。这也更合乎情理：只有那些已经存储到数据库或立刻就要存储到数据库的FileObject才有必要拥有一个Guid。</span></p>
<p>&nbsp;</p>
<p>用一个DefaultPersistence类型代替了BinaryPersistence和XmlPersistence。由于SoapFormatter和BinaryFormatter是近亲，而XmlSerializer跟他们是远亲；同时SoapFormatter和BinaryFormatter分别实现了文本文件序列化和二进制序列化，XmlSerializer就更不用出场了。因此现在不再使用XmlSerializer。</p>
<p>&nbsp;</p>
<p>PS：我国大多数县的人口为几万到几十万。目前，县里各种政府部门急需实现信息化网络化办公办事，但他们一般用不起那种月薪上万的开发者和高端软件公司。我注意到，一个县级政府部门日常应对的人群数量就是<span style="color: #ff0000;">万人</span>左右，甚至常常是千人左右。所以他们不需要太高端复杂的系统设计，用支持万人级别的数据库就可以了。另一方面，初级开发者也不能充分利用那些看似高端复杂的数据库的优势。做个小型系统而已，还是简单一点好。</p>
<p>所以我就想做这样一个小型文件数据库，我相信这会帮助很多人。能以己所学惠及大众，才是我们的价值所在。</p>

