using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstantDelete
{
    public class TimerData
    {
        long priventTime;
        int interval;
        string filePosition;
        
        public TimerData(string filePosition)
        {
            DateTime nowTime = DateTime.Now;
            this.priventTime = GetUnixTime(nowTime);
            this.filePosition = filePosition;

            this.interval = 60 * 60 * 24;
        } 
        public TimerData(string filePosition, int interval) : this(filePosition){
            this.interval = interval;
        }

        public TimerData(string filePosition,int interval,long time)
        {
            this.filePosition = filePosition;
            this.interval = interval;
            this.priventTime = time;
        }

        private static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static long GetUnixTime(DateTime targetTime)
        {
            // UTC時間に変換
            targetTime = targetTime.ToUniversalTime();

            // UNIXエポックからの経過時間を取得
            TimeSpan elapsedTime = targetTime - UNIX_EPOCH;

            // 経過秒数に変換
            return (long)elapsedTime.TotalSeconds;
        }

        public long getPriventTime()
        {
            return priventTime;
        }

        public int getInterval()
        {
            return interval;
        }

        public string getfilePosition()
        {
            return filePosition;
        }

        public void setPriventTime()
        {
            DateTime nowTime = DateTime.Now;
            this.priventTime = GetUnixTime(nowTime);

        }
    }

    public class saveTime
    {
        public long priventTime;
        public int interval;
        public string filePosition;

        /*
        public saveTime()
        {

        }
        */

        public void set(TimerData td)
        {
            priventTime = td.getPriventTime();
            interval = td.getInterval();
            filePosition = td.getfilePosition();
        }
    }
}
