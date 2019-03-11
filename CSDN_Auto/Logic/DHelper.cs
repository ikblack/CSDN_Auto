using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSDN_Auto
{
    public static class DHelper
    {
        static Random rand = new Random();
        public static string GetRandomComment()
        {
            string[] coms1 = new string[] {
                "资源不错，",
                "感觉还挺好用的，",
                "资源不错，",
                "不错很哈用，",
                "先下载看看，",
                "可以用，",
                "还行，",
                "下载速度飞起，",
                "下载太慢，",
                "精品，",
                "精辟，",
                "开始学习，",
                "好东西，",
                "不容易啊，",
                "免积分就好了，",
                "可惜不免积分，",
                "CSDN积分限制，",
                "恶心人的积分，",
                "积分机制烦，",
                "没积分了快，",
                "大爱，",
                "好哥们，",
                "东西不错，",
                "终于找到了，",
                "终于等到你，",
                "亲亲亲，",
                "好评，",
                "好人一生平安，",
            };

            string[] coms2 = new string[] {
                "谢谢。",
                "多谢！",
                "比心。",
                "激动！",
                "哈哈。",
                "呵呵。",
                "霍霍~",
                "不错~",
                "喜欢。。",
                "心动。",
                "开森~",
                "开心~",
                "开心！",
                "开森！",
                "喜欢~~",
                "爱你哦~",
                "么么哒~",
                "666"
            };
            return coms1[rand.Next(coms1.Length)] + coms2[rand.Next(coms2.Length)];
        }
    }
}
