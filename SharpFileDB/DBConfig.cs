using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// <see cref="FileDBContext"/>的配置信息。
    /// </summary>
    public class DBConfig
    {

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}", this.MaxLevelOfSkipList, this.ProbabilityOfSkipList, this.MaxSunkCountInMemory, this.LockTimeout);
            //return base.ToString();
        }

        /// <summary>
        /// <see cref="FileDBContext"/>的配置信息。
        /// </summary>
        public DBConfig()
        {
            this.MaxLevelOfSkipList = 32;
            this.ProbabilityOfSkipList = 0.5;
            this.MaxSunkCountInMemory = 10001;
            this.LockTimeout = new TimeSpan(0, 1, 0);
        }

        /// <summary>
        /// SkipList的最大层数。
        /// </summary>
        public int MaxLevelOfSkipList { get; set; }

        /// <summary>
        /// SkipList的随机阈值。
        /// </summary>
        public double ProbabilityOfSkipList { get; set; }

        /// <summary>
        /// <see cref="BlockCache.sunkBlocksInMomery"/>能存储的<see cref="Block"/>数目的最大值。如果达到最大值，就会清空<see cref="BlockCache.sunkBlocksInMomery"/>。
        /// </summary>
        public long MaxSunkCountInMemory { get; set; }

        /// <summary>
        /// 等带解锁的时间长度。
        /// </summary>
        public TimeSpan LockTimeout { get; set; }
    }
}
