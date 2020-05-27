using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Island
{
    /// <summary>
    /// 计算岛屿面积
    /// </summary>
    class Program
    {
        static int xMax,yMax;
        static void Main(string[] args)
        {
            Console.WriteLine("输入地图大小");
            Console.WriteLine("输入地图横坐标");
            xMax = 2048;
            //if (xMax < 9 || xMax > 50)
            //{ xMax = 50;
            //    Console.WriteLine("输入的数值太大或者太小，使用默认值50");
            //}
            Console.WriteLine("输入地图纵坐标");
            yMax = 1024;
            //if (yMax < 9 || yMax > 50)
            //{
            //    yMax = 50;
            //    Console.WriteLine("输入的数值太大或者太小，使用默认值50");
            //}
            for (int i = 0; i < 100; i++)
            {
                int[,] map = CreateIsLands(xMax, yMax);
            //PrintMap(map);
                ImageWarp.DrawIsland(i,map);
            }          
            //GetIsLandsArea(0, 0, map);
        }

        /// <summary>
        /// 生成随机岛屿
        /// </summary>
        /// <param name="mx"></param>
        /// <param name="my"></param>
        /// <returns></returns>
        static int[,] CreateIsLands(int mx,int my)
        {
            int[,] Islands = new int[mx, my];
            //int IsLandsNum = mx < my ? mx / 2 : my / 2;//岛屿总数量为地图横纵坐标中较小值的一半
            int IsLandsNum = 19;
            Console.WriteLine("预计生成随机岛屿数量为{0}个，生成岛屿时，使用CanCreatePoint函数来进行判断，避免和其他岛屿合并", IsLandsNum);
            int[,] next = { 
                            {-1,0},//上
                            {1,0 },//下
                            {-1,-1},//左上
                            {1,-1},//左下
                            {0,-1},//左
                            {1,1 },//右下
                            {0,1 },//右
                            {-1,1}//右上
            };
            Random rd = new Random();
            int rx = 0, ry = 0;
            //在for循环中依次生成岛屿
            for(int i=1;i<=IsLandsNum;i++)
            {
                node[] queue = new node[mx*my];//生成用于扩展的队列
                int head = 0, tail = 0;
                int[,] currentIsland = new int[mx, my];
                rx = rd.Next(mx);
                ry = rd.Next(my);
                //当随机的岛屿中心点在地图上已经为大于0的数时，则表示次点已经有岛屿存在，需要重新随机生成一个
                while(CanCreatePoint(rx,ry,Islands,currentIsland)==false)
                {   rx = rd.Next(mx);
                    ry = rd.Next(my);
                }
                Islands[rx, ry] = i;//将这个点标记为中心点,使用广度优先算法向周围扩散以生成岛屿，需要注意避免与其他岛屿相连，需判断下一点的下一点是否存在岛屿
                currentIsland[rx, ry] = 1;//将当前点添加当前岛屿点数组中
                //我们需要一个岛屿点数的随机数，表示从当前中心点扩展的点数，需要一个队列，来存储扩展的点数
                int MaxPoint = rd.Next(xMax * yMax / 40,xMax * yMax / 20);
                int pointNum = 0;
                queue[tail] = new node();
                queue[tail].x = rx;
                queue[tail].y = ry;
                queue[tail].f = 0;
                queue[tail].step = 0;
                tail++;
                int tx, ty;
                while(head<tail)
                {
                    Random rdd = new Random();
                    for (int k = 0; k < 8; k++)
                    {
                        int choose = rdd.Next(0,8);
                        if (choose >= 8) continue;
                        //Console.WriteLine("choose:{0}", choose);
                        tx = queue[head].x + next[choose, 0];
                        ty = queue[head].y + next[choose, 1];
                        //判断是否越界了，如果越界了则判断下一个方向
                        if (tx < 0 || tx >= xMax || ty < 0 || ty >= yMax)
                            continue;
                        //判断下一点是否能够创建岛屿点
                        if (CanCreatePoint(tx, ty, Islands, currentIsland) ==true)
                        {
                            queue[tail] = new node();
                            queue[tail].x = tx;
                            queue[tail].y = ty;
                            queue[tail].f = head;
                            queue[tail].step = queue[head].step + 1;
                            tail++;
                            Islands[tx, ty] = i;
                            currentIsland[tx,ty] = 1;//将当前点添加当前岛屿点数组中
                            pointNum++;
                        }
                    }
                    //退出循环条件，当扩展点数已经大于或者等于最大点数
                    if (pointNum >= MaxPoint)
                    {
                        Console.WriteLine("第{0}号岛屿，预设最大面积为{1}", i, MaxPoint);
                        break;
                    }
                    head++;
                }
            }
            return Islands;
        }

        /// <summary>
        /// 计算岛屿面积函数：
        /// 要求找出所有存在的岛屿，并计算每一个岛屿的面积大小
        /// 思路，要遍历地图上所有的点，当遇到一个点是岛屿时，判断该点是否为已经计算过的岛屿，如果是则跳过，如果不是则从该点进行广度优先搜索
        /// 当遇到海洋时，跳过操作，当遇到陆地时，将面积加一，将该点计入已计算岛屿，当从该点扩展的所有点已统计完成，跳出该岛屿的计算，等待遍历
        /// 到下一个未计算过的岛屿点，再次使用广度优先搜索来计算新的岛屿，直到所有点循环完毕。
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="Map"></param>
        static void GetIsLandsArea(int startX,int startY,int[,]Map)
        {
            int[,] hisIslands = new int[xMax, yMax];//创建一个数组，大小和地图相同，用于记录岛屿点是否被计算过。
            int area = 0;//当前岛屿面积
            int IslandNo = 0;
            int[,] next = { {0,1 },//右
                            {1,1 },//右下
                            {1,0 },//下
                            {1,-1},//左下
                            {0,-1},//左
                            {-1,-1},//左上
                            {-1,0},//上
                            {-1,1}//右上
            };
            int queueLen = (xMax < yMax ? xMax : yMax) + 3;//队列长度,每个岛屿的面积不会超过xy坐标中较小中的那一个加3.这个是生成岛屿的时候设置的面积大小上限
            //使用for循环遍历所有点
            for (int x=0;x<xMax;x++)
            {
                for(int y=0;y<yMax;y++)
                {
                    //如果当前点是岛屿点，并且没有计算过,这里不需要判断是否越界，因为范围已经限制在有效范围内
                    if(Map[x,y]>0&&hisIslands[x,y]==0)
                    {
                        IslandNo++;//设置岛屿编号
                        area++;//当前岛屿面积加一
                        hisIslands[x, y] = 1;//将该点标注为已计算                        
                        node[] queue = new node[queueLen];//创建队列用于进行广度优先搜索
                        int head = 0, tail = 0;//定义头尾变量
                        queue[tail] = new node();
                        queue[tail].x = x;
                        queue[tail].y = y;
                        queue[tail].step = 0;
                        queue[tail].f = 0;
                        tail++;
                        int tx, ty;//下一个点可能的坐标
                        while(head<tail)//开始扩展点
                        {
                         for(int k=0;k<8;k++)//向8个方向进行扩展
                            {
                                tx = queue[head].x + next[k, 0];
                                ty = queue[head].y + next[k, 1];
                                if (tx < 0 || tx >= xMax || ty < 0 || ty >= yMax||Map[tx, ty] == 0||hisIslands[tx,ty]==1)//如果下一个点越界了,或者下一个点为海洋，或者下一点已经统计过了，则跳过这个点
                                    continue;
                                //如果当前点没有被跳过，说明这个点是当前岛屿没有被计算过面积的点
                                queue[tail] = new node();
                                queue[tail].x = tx;
                                queue[tail].y = ty;
                                queue[tail].step = queue[head].step+1;
                                queue[tail].f = head;
                                tail++;//将队尾后移，以便下一个数据入队
                                hisIslands[tx, ty] = 1;//当前点标记为已计算
                                area++;//当前岛屿面积加一
                            }
                            head++;//当8个方向都已经扩展完毕，说明当前点已经扩展完成，将该点出队，开始下一个点的扩展 
                        }
                        if(area>0)
                        {
                            Console.WriteLine("找到第{3}号岛屿,当前岛屿坐标为{0},{1}.当前岛屿面积为{2}", x, y, area,IslandNo);
                            area = 0;//将面积清空以便下个岛屿使用
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 判断是否已经存在岛屿,需判断当前点以及以当前点八个方向是否存在岛屿，避免岛屿相连
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        static bool isHasIsland(int x,int y,int [,]map)
        {
            int[,] next ={  {0,1 },//右
                            {1,1 },//右下
                            {1,0 },//下
                            {1,-1},//左下
                            {0,-1},//左
                            {-1,-1},//左上
                            {-1,0},//上
                            {-1,1}//右上
            };
            int tx=0, ty=0,Islandp=0;
            //判断当前点是否有岛屿存在，如果有直接返回真
            if (map[x, y] > 0)
            { return true; }
            else
            {
                //判断以当前点为中心8个方向是否有岛屿存在，如果有，则表明，如果在此点添加一个岛屿点会与其他岛屿相连接
                for (int i = 0; i < 8; i++)
                {
                    tx = x + next[i, 0];
                    ty = y + next[i, 1];
                    //判断是否越界了，如果越界了则判断下一个方向
                    if (tx < 0 || tx >= xMax || ty < 0 || ty >= xMax)
                        continue;
                    //如果有一个方向有其他岛屿点，则表示会造成岛屿向连接
                    if (map[tx, ty] > 0)
                        Islandp++;
                }
                //根据相连岛屿的个数，返回结果
                if (Islandp > 0)
                    return true;
                else return false;
            } 
        }

       
        /// <summary>
        /// 重新设计一个函数，用于判断生成岛屿是否会与其他岛屿合并
        /// 要求，根据当前点，判断向其他空白方向扩展时，不与其他岛屿合并，并能够在判断时，忽略当前岛屿的点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="map"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        static bool CanCreatePoint(int x,int y,int[,]map,int[,]current)
        {
            if (x >= xMax  || y >= yMax)
            {
                return false;
            }
            
            int[,] next ={  {0,1 },//右
                            {1,1 },//右下
                            {1,0 },//下
                            {1,-1},//左下
                            {0,-1},//左
                            {-1,-1},//左上
                            {-1,0},//上
                            {-1,1}//右上
            };
            int tx = 0, ty = 0, Islandp = 0;
            //判断当前点是否有岛屿存在，如果有直接返回假，表示不能在此创建
            if (map[x, y] > 0)
            { return false; }
            else
            {
                //判断以当前点为中心8个方向是否有岛屿存在，如果有，则表明，如果在此点添加一个岛屿点会与其他岛屿相连接，此处应该忽略本岛屿的点
                for (int i = 0; i < 8; i++)
                {
                    tx = x + next[i, 0];
                    ty = y + next[i, 1];
                    //判断是否越界了，如果越界了则判断下一个方向
                    if (tx < 0 || tx >= xMax || ty < 0 || ty >= yMax)
                        continue;
                    //如果当前方向是本岛屿的点，跳过当前方向判断，可以和本岛屿的点进行合并，对下个方向进行判断
                    if (current[tx, ty] == 1)
                        continue;
                    //如果有一个方向有其他岛屿点，则表示会造成和其他岛屿相连接
                    if (map[tx, ty] > 0)
                        Islandp++;
                }
                //根据相连岛屿的个数，返回结果
                if (Islandp > 0)
                    return false;
                else return true;
            }
        }

        /// <summary>
        /// 打印岛屿
        /// </summary>
        /// <param name="Map"></param>
        static void PrintMap(int[,] Map)
        {
            Console.WriteLine();
            for (int x = 0; x < xMax; x++)
            {
                for (int y = 0; y < yMax; y++)
                {
                    int p = Map[x, y];
                    if (p < 10)
                    {
                        Console.Write(" ");
                        Console.Write(p);
                    }
                    else
                    {
                        Console.Write(p);
                    }
                    //switch (p)
                    //{
                    //    case 0://海洋
                    //        Console.Write("■");
                    //        break;
                    //    case 1://岛屿
                    //        Console.Write("▓");
                    //        break;                       
                    //}
                }
                Console.WriteLine();
            }
            Console.WriteLine("***********************岛屿打印完毕****************************");
        }
    }
    public class node
    {
        public node()
        {
            x = 0;
            y = 0;
            f = 0;
            step = 0;
        }
        public int x;//横坐标
        public int y;//纵坐标
        public int f;//父节点所在队列位置
        public int step;//扩展步数        
    }
}
