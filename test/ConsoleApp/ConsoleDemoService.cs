using Common.Dtos;
using Common.Entities;
using Common.Extensions;
using Common.Extensions.Mapping;
using Common.Helpers;
using ConsoleApp.AOP;
using ConsoleApp.Dtos;
using ConsoleApp.HttpClients;
using ConsoleApp.Models;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUglify.Helpers;
using Parakeet.Net.GrpcService;
using System.Collections;
using System.Collections.Concurrent;
using System.Data;
using System.Net.Http.Headers;
using System.Text;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace Parakeet.Net.ConsoleApp;

public class ConsoleDemoService : ITransientDependency
{
    private readonly IConfiguration _configuration;
    private AbpSequentialGuidGeneratorOptions _abpSequentialGuidGeneratorOptions;
    private readonly IGuidGenerator _guidGenerator;

    public ConsoleDemoService(IConfiguration configuration, IOptions<AbpSequentialGuidGeneratorOptions> abpSequentialGuidGeneratorOptions, IGuidGenerator guidGenerator)
    {
        _configuration = configuration;
        _guidGenerator = guidGenerator;
        _abpSequentialGuidGeneratorOptions = abpSequentialGuidGeneratorOptions.Value;
    }

    public async Task RunAsync()
    {
        //netcore默认为utf-8 支持多种编码
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        #region 

        #endregion

        #region 二面 业务
        {
            ////1、二维数组 存放 学生 成绩    合并学生 各科成绩  有重复的 用分号隔开

            //var students = new List<StudentRecord>();

            //var student1 = new StudentRecord { Name = "张三", Shuxue = "80", YuWen = "70" };
            //var student2 = new StudentRecord { Name = "李四", Shuxue = "81", YuWen = "60" };
            //var student3 = new StudentRecord { Name = "张三", YuWen = "50" };
            //students.Add(student1);
            //students.Add(student2);
            //students.Add(student3);

            //var result = GetMergeRecord(students, record => record.Name);
            //Console.WriteLine($"第1种方式:直接使用集合group");
            //foreach (var record in result)
            //{
            //    Console.WriteLine($"{record.Name}:{record.Shuxue} {record.YuWen}");
            //}
            //Console.ReadLine();

            //List<StudentRecord> GetMergeRecord(List<StudentRecord> records, Func<StudentRecord, string> express)
            //{
            //    return records.GroupBy(express).Select(x => new StudentRecord
            //    {
            //        Name = x.Key,
            //        Shuxue = $"{string.Join(";", x.Select(y => y.Shuxue))}",
            //        YuWen = $"{string.Join(";", x.Select(y => y.YuWen))}"
            //    }).ToList();
            //}


            //student1.AddSubject(new SubjectRecord { Name = "Yuwen", Score = "70" });
            //student1.AddSubject(new SubjectRecord { Name = "Yuwen", Score = "50" });
            //student3.AddSubject(new SubjectRecord { Name = "Shuxue", Score = "80" });
            //student2.AddSubject(new SubjectRecord { Name = "Shuxue", Score = "81" });
            //student2.AddSubject(new SubjectRecord { Name = "Yuwen", Score = "60" });

            //Console.WriteLine($"第1.5种方式:使用子表");
            //foreach (var groupItem in students.GroupBy(x => x.Name).ToList())
            //{
            //    var student = groupItem.First();
            //    if (groupItem.Count() > 1)
            //    {
            //        student.AddSubjects(groupItem.Where(x=>x.Id!=student.Id).SelectMany(x=>x.Subjects).ToList());
            //    }
            //    Console.WriteLine(student.GetSubjectsDisplay());
            //}
            //Console.ReadLine();



            ////改进：使用成员方法 动态添加学科与分数
            //student1.AddScore("Yuwen", "89");
            //student2.AddScore("Yuwen", "89");
            //student3.AddScore("Yuwen", "89");
            //student1.AddScore("Yuwen", "80");
            //student2.AddScore("Yuwen", "81");
            //student1.AddScore("Yuwen", "83");

            //student1.AddScore("ShuXue", "99");
            //student2.AddScore("ShuXue", "99");
            //student3.AddScore("ShuXue", "99");
            //student1.AddScore("ShuXue", "90");
            //student2.AddScore("ShuXue", "91");
            //student1.AddScore("ShuXue", "93");


            //student1.AddScore("English", "100");


            //Console.WriteLine($"第二种方式:使用字典数组展示学生合并后的成绩:");

            //var group = students.GroupBy(x => x.Name).ToList();
            //foreach (var groupItem in group)
            //{
            //    var first = groupItem.First();
            //    foreach (var student in groupItem)
            //    {
            //        if (student.Id != first.Id)
            //        {
            //            student.SubjectScores.ForEach(x => first.AddScore(x.Key, string.Join(";", x.Value)));
            //        }
            //    }
            //    Console.WriteLine(first);
            //}
            //Console.ReadLine();


        }

        #endregion

        #region 一面 算法

        #region IsAnagram 同位数练习
        {

            ////Given two strings s and t, return true if t is an anagram of s, and false otherwise.
            ////An Anagram is a word or phrase formed by rearranging the letters of a different word or phrase, typically using all the original letters exactly once.
            ////Requirement: You cannot use methods in linq and class libraries.
            ////Example 1:
            ////Input: s = "anagram", t = "nagaram"
            ////Output: true

            ////Example 2:
            ////Input: s = "rat", t = "car"
            ////Output: false

            //方法1、使用原生 IsAnagram 直接题意判断 复杂度太高，代码太low了
            //方法2、//循环一遍s 每一个字符出现后，直接去t里面splice掉第一个出现的位置，然后循环s之后，t的长度应该变为0 面试官启发
            //方法3、//循环一遍t 每一个tWord在s中indexOf操作找到不为空且index不同  自己想的 但不知道能使用index 题目说不能使用类库和linq

            //var parameterS = Console.ReadLine();
            //var parameterT = Console.ReadLine();

            //var isAnagram = IsAnagram(parameterS, parameterT);
            //Console.WriteLine($"{parameterT} is an anagram of {parameterS} :{isAnagram}");

            //Console.ReadKey();
            bool IsAnagram(string s, string t)
            {
                //题意说每个字母只使用一遍，例如Example 1中原字符串中a出现了三次，那变位数也只能出现三次，并且变位数长度一定等于原字符串长度
                //不能用任何linq和类库，但是可以使用自带的dictionary，笨办法，循环多次，这题挺有意思，细节挺多
                var result = s.Length == t.Length;
                if (result)
                {

                    #region 方法三、 可以使用indexOf等字符串函数情况下 自己想的 循环一遍t 每一个tWord在s中indexOf操作找到不为空且index不同  自己想的 但不知道能使用index 题目说不能使用类库和linq
                    var preIndexs = new List<int> { -1 };
                    foreach (var letter in t)
                    {
                        var index = s.IndexOf(letter);
                        preIndexs.Add(index);
                        result = !preIndexs.Contains(index);
                    }


                    #endregion

                    #region 方法二、面试官启发 循环一遍s 每一个字符出现后，直接去t里面splice掉第一个出现的位置，然后循环s之后，t的长度应该变为0 面试官启发

                    foreach (var letter in s)
                    {
                        var index = t.IndexOf(letter);
                        t = t.Remove(index, 1);
                    }
                    result = t.Length == 0;


                    #endregion

                    #region  方法1、使用原生 IsAnagram 直接题意判断 复杂度太高，代码太low了

                    //var sLetterDic = GetLetterDic(s);
                    //var tLetterDic = GetLetterDic(t);
                    ////foreach (var tLetter in t)
                    ////{
                    ////    result = sLetterDic.ContainsKey(tLetter) && sLetterDic[tLetter] == tLetterDic[tLetter];
                    ////    if (!result)
                    ////    {
                    ////        break;
                    ////    }
                    ////}
                    //foreach (var dic in sLetterDic)
                    //{
                    //    result = tLetterDic.ContainsKey(dic.Key);
                    //    if (result)
                    //    {
                    //        if (sLetterDic[dic.Key] != tLetterDic[dic.Key])
                    //        {
                    //            result = false;
                    //            break;
                    //        }
                    //    }
                    //}

                    #endregion
                }
                return result;
            }

            ConcurrentDictionary<char, int> GetLetterDic(string str)
            {
                var letterDic = new ConcurrentDictionary<char, int>();
                foreach (char sLetter in str)
                {
                    if (letterDic.ContainsKey(sLetter))
                    {
                        letterDic[sLetter]++;
                    }
                    else
                    {
                        letterDic.TryAdd(sLetter, 1);
                    }
                }
                return letterDic;
            }


        }

        #endregion

        #region 排序等基础数据结构操作算法
        {
            //BubbleSort();

            /// <summary>
            /// 冒泡排序
            /// 比较相邻的元素。如果第一个比第二个大，就交换他们两个。
            /// 对每一对相邻元素作同样的工作，从开始第一对到结尾的最后一对。这步做完后，最后的元素会是最大的数。
            /// 针对所有的元素重复以上的步骤，除了最后一个。
            /// 持续每次对越来越少的元素重复上面的步骤，直到没有任何一对数字需要比较。
            /// </summary>
            void BubbleSort()
            {
                var array = new int[5]; //Enumerable.Range(1, 5).ToArray();

                // 创建一个Random对象用于生成随机数
                var random = new Random();
                // 使用for循环填充数组随机数
                for (int i = 0; i < array.Length; i++)
                {
                    // 生成1到100之间的随机数（Next方法的上限是排外的，所以用101来包含100）
                    array[i] = random.Next(1, 101);
                }

                Console.WriteLine($"intArray 排序前:{string.Join(",", array)}");

                int temp = 0;
                bool swapped;
                for (int i = 0; i < array.Length; i++)
                {
                    swapped = false;
                    for (int j = 0; j < array.Length - 1 - i; j++)
                    {
                        if (array[j] > array[j + 1])
                        {
                            temp = array[j];
                            array[j] = array[j + 1];
                            array[j + 1] = temp;
                            if (!swapped)
                                swapped = true;
                        }
                    }
                    if (!swapped)
                        return;
                }

                Console.WriteLine($"intArray 排序后:{string.Join(",", array)}");
            }

            /// <summary>
            /// 二分法查找
            ///  // 没有找到目标值}//调用int[] nums = {1, 2, 3, 4, 5, 6, 7, 8, 9};
            ///  int target = 6;
            ///  int index = BinarySearch(nums, target);
            ///  Console.WriteLine(index);
            /// </summary>
            /// <param name="nums"></param>
            /// <param name="target"></param>
            /// <returns></returns>
            int BinarySearch(int[] nums, int target)
            {
                var findIndex = -1;
                int leftIndex = 0;
                int rightIndex = nums.Length - 1;
                int midIndex = rightIndex / 2;
                while (leftIndex <= rightIndex)
                {
                    if (nums[midIndex] == target)
                    {
                        findIndex = midIndex;
                        break;
                    }
                    else if (nums[midIndex] < target)
                    {
                        leftIndex = midIndex + 1;
                    }
                    else
                    {
                        rightIndex = midIndex - 1;
                    }
                    midIndex = leftIndex + (rightIndex - leftIndex) / 2;
                }
                Console.WriteLine($"findIndex={findIndex}");
                return findIndex;
            }

            /// <summary>
            /// //调用 string haystack = "DOTNET开发";
            /// string needle = "NET";
            /// int index = StrStr(haystack, needle);
            /// Console.WriteLine(index);
            /// </summary>
            /// <param name="haystack"></param>
            /// <param name="needle"></param>
            /// <returns></returns>
            int SubStr(string haystack, string needle)
            {
                //字串判断 是不是可以使用splice方法 然后判断母串前后的长度差，如果有差异说明直接字串，indexof就能找出位置了

                return haystack.IndexOf(needle);

                if (string.IsNullOrEmpty(needle))
                {
                    return 0;
                }
                int n = haystack.Length;
                int m = needle.Length;
                if (n < m)
                {
                    return -1;
                }
                for (int i = 0; i <= n - m; i++)
                {
                    int j;
                    for (j = 0; j < m; j++)
                    {
                        if (haystack[i + j] != needle[j])
                        {
                            break;
                        }
                    }
                    if (j == m)
                    {
                        return i;
                    }
                }
                return -1;
            }

            /// <summary>
            /// 选择排序
            /// 首先在未排序序列中找到最小（大）元素，存放到排序序列的起始位置。
            /// 再从剩余未排序元素中继续寻找最小（大）元素，然后放到已排序序列的末尾。
            /// </summary>
            /// <param name="nums"></param>
            void SelectionSort(int[] nums)
            {
                for (int i = 0; i < nums.Length - 1; i++)
                {
                    int minIndex = i;
                    for (int j = i + 1; j < nums.Length; j++)
                    {
                        if (nums[j] < nums[minIndex])
                        {
                            minIndex = j;
                        }
                    }
                    if (minIndex != i)
                    {
                        int temp = nums[i];
                        nums[i] = nums[minIndex];
                        nums[minIndex] = temp;
                    }
                }
            }


            //InsertionSort([5, 4, 3, 2, 1]);
            //Console.ReadLine();

            /// <summary>
            /// 插入排序
            /// </summary>
            void InsertionSort(int[] array)
            {
                Console.WriteLine($"排序前:{string.Join(",", array)}");
                //for (int i = 1; i < array.Length; i++)
                //{
                //    int temp = array[i];
                //    for (int j = i - 1; j >= 0; j--)
                //    {
                //        if (array[j] > temp)
                //        {
                //            array[j + 1] = array[j];
                //            array[j] = temp;
                //        }
                //        else
                //        {
                //            break;
                //        }
                //    }
                //    Console.WriteLine($"第{i}次排序后：{string.Join(",", array)}");
                //}

                for (int i = 0; i < array.Length; i++)
                {
                    var temp = array[0];
                    for (int j = 1; j < array.Length - i; j++)
                    {
                        if (array[j] <= temp)
                        {
                            array[j - 1] = array[j];
                            array[j] = temp;
                        }
                    }
                    Console.WriteLine($"第{i}次排序后：{string.Join(",", array)}");

                }

                Console.WriteLine($"排序后：{string.Join(",", array)}");
            }


            //KaotiFive();//Matlab 考题5 采取逆向思维
            //Console.ReadLine();


            //% 问题：水手、猴子和椰子问题：五个水手带了一只猴子来到南太平洋的一个荒岛上，发现那里有一大堆椰子.由于旅途
            //% 的颠簸，大家都很疲倦，很快就入睡了.第一个水手醒来后，把椰子平分成五堆，将多余的一只给了猴子，他私藏了
            //% 一堆后便又去睡了.第二、第三、第四、第五个水手也陆续起来，和第一个水手一样，把椰子分成五堆，恰多一只
            //% 给猴子，私藏一堆，再去入睡.天亮以后，大家把余下的椰子重新等分成五堆，每人分一堆，正好余一只再给猴子.
            //% 试问原先最少有多少只椰子？
            //% 提示：采取逆向思维的方法，从后往前推断，用matlab编写程序求出原先的椰子数目，建立模型，循环
            long KaotiFive()
            {
                long i = 1;
                long k = 1;
                long x = 1;
                for (i = 1; i < 100000000000000; i++)
                {
                    x = i;
                    for (k = 1; k <= 6; k++)
                    {

                        x = 5 * x + 1;
                        if (x % 4 == 0)
                        {
                            x = x / 4;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (k >= 6)
                    {
                        Console.WriteLine($"最后一次每个水手分到:{i}个椰子");
                        break;
                    }
                }
                Console.WriteLine($"原来的椰子数目至少为:{x}个");
                return x;
            }

            //二叉树中序遍历
            //var root = new TreeNode(1, null, new TreeNode(2, new TreeNode(3)));
            //var nodes = new List<int>();
            //root.InorderTraversal(root, nodes);
            //Console.WriteLine($"nodes 中序遍历后:{string.Join(",",nodes)}");
            //Console.ReadLine();


            //回文字符串 验证 使用堆栈呀
            //ValidateHuiwenStr();
            bool ValidateHuiwenStr(string huiwenStr = "abcdedcba")
            {
                //var stack=new Stack<char>();
                //foreach (var item in huiwenStr)
                //{
                //    stack.Push(item);
                //}
                var result = false;
                for (var index = 0; index < huiwenStr.Length; index++)
                {
                    if (index > huiwenStr.Length / 2)
                    {
                        break;
                    }
                    result = huiwenStr[index] == huiwenStr[huiwenStr.Length - index - 1];
                }
                return result;
            }

        }

        #endregion

        #region  leetCode 实践
        {

            //给你一个 非空 整数数组 nums ，除了某个元素只出现一次以外，其余每个元素均出现两次。找出那个只出现了一次的元素。

            //你必须设计并实现线性时间复杂度的算法来解决此问题，且该算法只使用常量额外空间。


            //这种思路 牛逼了 去重累加*2-非去重累加=原本数组单着这个数
            int FindOnceNum(int[] nums)
            {            
                return nums.Distinct().Sum(x=>x) * 2 - nums.Sum(x => x);
            }


            ////给定一个字符串 s ，通过将字符串 s 中的每个字母转变大小写，我们可以获得一个新的字符串。

            ////返回 所有可能得到的字符串集合 。以 任意顺序 返回输出。

            //Console.WriteLine($"大小写字串:{string.Join(",", GetStrList().Distinct())}");
            //Console.ReadLine();
            List<string> GetStrList(List<string> result = null, string str = "a1b2", int index = 0)
            {
                //var dic = new Dictionary<int, List<string>>();
                //var charDic = new Dictionary<int, List<char>>();
                result ??= new List<string>();
                var sbUpper = new StringBuilder(str);
                var sbLower = new StringBuilder(str);
                var item = str[index];
                if (str[index] >= 'a' && str[index] <= 'z' || str[index] >= 'A' && str[index] <= 'Z')
                {
                    sbUpper[index] = char.ToUpper(str[index]);
                    sbLower[index] = char.ToLower(str[index]);
                    result.Add(sbUpper.ToString());
                    result.Add(sbLower.ToString());
                }

                if (index + 1 < str.Length)
                {
                    GetStrList(result, sbUpper.ToString(), index + 1);
                    GetStrList(result, sbLower.ToString(), index + 1);
                }
                return result;
            };


            //一个 2D 网格中的 峰值 是指那些 严格大于 其相邻格子(上、下、左、右)的元素。
            //给你一个 从 0 开始编号 的 m x n 矩阵 mat ，其中任意两个相邻格子的值都 不相同 。找出 任意一个 峰值 mat[i][j] 并 返回其位置[i, j] 。
            //你可以假设整个矩阵周边环绕着一圈值为 - 1 的格子。
            //要求必须写出时间复杂度为 O(m log(n)) 或 O(n log(m)) 的算法

            //理论：可以根据中间位置找 上下左右比较，如果找到比自己大的就直接把位置切换到大的一个数 递归 上下左右，一直找到第一个返回即可



            //int[,] mattrix = { { 10, 20, 15 }, { 21, 30, 14 }, { 7, 16, 32 } };//{ { 1, 4 }, { 3, 2 } };

            //FindPeakGrid(mattrix);
            //Console.ReadLine();
            //0(m*n) 去找
            int[,] FindPeakGrid(int[,] mat)
            {
                int m = mat.GetLength(0);
                var n = mat.GetLength(1);
                var newMat = new int[m + 2, n + 2];
                for (var i = 0; i < newMat.GetLength(0); i++)
                {
                    for (var j = 0; j < newMat.GetLength(1); j++)
                    {
                        if (i == 0 || j == 0 || i == m + 1 || j == n + 1)
                        {
                            newMat[i, j] = -1;
                        }
                        else
                        {
                            newMat[i, j] = mat[i - 1, j - 1];
                        }
                    }
                }

                Console.WriteLine();
                Console.WriteLine($"打印填充默认数据的二维数组：");
                var displayStr = string.Empty;
                for (int i = 0; i < newMat.GetLength(0); i++)
                {
                    for (int j = 0; j < newMat.GetLength(1); j++)
                    {
                        Console.Write(newMat[i, j] + " ");
                        if (!(i == 0 || j == 0 || i == m + 1 || j == n + 1))
                        {
                            if (newMat[i, j] > newMat[i - 1, j] && newMat[i, j] > newMat[i, j - 1] && newMat[i, j] > newMat[i + 1, j] && newMat[i, j] > newMat[i, j + 1])
                            {
                                displayStr = $"{displayStr} [{i - 1},{j - 1}]:{newMat[i, j]}";

                                //找到就可以直接return
                            }
                            //if (newMat[i, j] > newMat[i-1,j]+ newMat[i, j-1]+ newMat[i+1, j]+ newMat[i, j+ 1])
                            //{
                            //    displayStr = $"{displayStr} [{i - 1},{j - 1}]:{newMat[i, j]}";

                            //    //找到就可以直接return
                            //}
                        }
                    }
                    Console.WriteLine();
                }

                Console.WriteLine();
                Console.WriteLine($"原二维数组：");

                for (int i = 0; i < mat.GetLength(0); i++)
                {
                    for (int j = 0; j < mat.GetLength(1); j++)
                    {
                        Console.Write(mat[i, j] + " ");
                    }
                    Console.WriteLine();
                }



                Console.WriteLine();
                Console.WriteLine($"打印原二维数组满足条件的下标与值：");
                Console.WriteLine($"{displayStr}");


                return newMat;
            }


            ////目的是求最大和 而不是每个子序列的和然后排序得来 不能使用笨办法
            //int[] nums = { -2, 1, -3, 4, -1, 2, 1, -5, 4 };
            //int maxSum = FindMaximumSubarraySum(nums);
            //Console.WriteLine("Maximum contiguous sum is " + maxSum);


            int FindMaximumSubarraySum(int[] nums)
            {
                int maxSoFar = nums[0];//初始化最大和为数组的第一个元素
                int currMax = nums[0];//当前子数组和

                foreach (var item in nums)
                {
                    currMax = Math.Max(item, currMax + item);//currMax + item 代表连续的几个数相加 转换为与前一个和相加大于自己
                    maxSoFar = Math.Max(maxSoFar, currMax);//更新最大和
                }

                return maxSoFar;
            }

            //两数之和 转换思路为 减法 寻找剩余字段是否再之前遍历的数据里  
            //保存之前遍历数据的数组下标 就需要一个字典了
            //int[] arrayNum = [2, 7, 11, 15];
            //var target = 9;
            //Console.WriteLine($"和为{target}的下标:{string.Join(",", FindIndexs(arrayNum, target))}");
            //Console.ReadLine();

            IList<int> FindIndexs(int[] nums, int target)
            {
                //初始化一个字典存放数组 值与在数组中的下标数据
                Dictionary<int, int> numIndexDic = new Dictionary<int, int>();
                var indexs = new List<int>();
                for (int i = 0; i < nums.Length; i++)
                {
                    if (!numIndexDic.ContainsKey(nums[i]))
                    {
                        numIndexDic.Add(nums[i], i);
                    }

                    if (i > 0)
                    {
                        var findNum = target - nums[i];
                        if (numIndexDic.ContainsKey(findNum))
                        {
                            //找到了
                            var index = numIndexDic[findNum];
                            indexs.Add(index);
                            indexs.Add(i);
                            break;
                        }
                    }

                }
                return indexs;
            }



            //string[] arrayStr = ["eata", "teaa", "tana", "atea", "nata", "bata"];
            //GroupAnagrams(arrayStr);
            //Console.ReadLine();
            List<List<string>> GroupAnagrams(string[] strArray)
            {
                Dictionary<string, List<string>> dics = new Dictionary<string, List<string>>();

                foreach (var str in strArray)
                {
                    char[] chr = str.ToCharArray();
                    Array.Sort(chr);
                    var key = new string(chr);

                    //dic的key 对应value：存储key相同的原始单词列表，取出list，如果没有当前单词的key 则创建空list
                    if (dics.ContainsKey(key))
                    {
                        dics[key].Add(str);
                    }
                    else
                    {
                        dics.Add(key, new List<string> { str });
                    }
                }
                foreach (var dic in dics)
                {
                    Console.WriteLine($"{dic.Key}:{string.Join(",", dic.Value)}");
                }
                return dics.Select(x => x.Value).ToList();
            }

            //给定一个未排序的整数数组 nums ，找出数字连续的最长序列（不要求序列元素在原数组中连续）的长度。
            //请你设计并实现时间复杂度为 O(n) 的算法解决此问题。
            //var arrayNum = new[] { 100, 4, 200, 1, 3, 2 };
            //var lenth = LongestConsecutive(arrayNum);
            //Console.WriteLine($"连续字序列度{lenth}");
            //Console.ReadLine();
            int LongestConsecutive(int[] nums)
            {
                var count = 1;
                var maxCount = 1;
                nums = nums.OrderBy(x => x).ToArray();
                var pre = nums[0];
                //var consecutiveList = new List<int>();
                foreach (var num in nums)
                {
                    if (num - pre == 1)
                    {
                        //consecutiveList.Add(num);
                        count++;
                    }
                    else
                    {
                        maxCount = Math.Max(maxCount, count);
                        count = 1;
                    }
                    pre = num;
                }
                return maxCount;
            }


            ////leetcode原题：一只青蛙一次可以跳上1级台阶，也可以跳上2级台阶。求该青蛙跳上一个 10 级的台阶总共有多少种跳法
            //var total = GetJumpStep(10);
            //Console.ReadLine();

            //递归 空间复杂度
            int GetJumpNumber(int level)
            {
                var step = 0;
                if (level == 0)
                {
                    step = 1;
                }
                if (level <= 2)
                {
                    step = level;
                }
                if (level >= 3)
                {
                    step = GetJumpNumber(level - 1) + GetJumpNumber(level - 2);//循环体空间复杂度不行 改用动态规划
                }
                return step;
            }


            //转换为动态规划 计算 
            int GetJumpStep(int level)
            {
                var dp = new int[1, level + 1];

                dp[0, 0] = 0;//没有台阶的时候是不需要跳的
                dp[0, 1] = 1;
                dp[0, 2] = 2;

                //后面的动态计算出来
                for (int i = 3; i <= level; i++)
                {
                    dp[0, i] = dp[0, i - 1] + dp[0, i - 2];
                    Console.WriteLine($"dp[0,{i}]={dp[0, i]}");
                }
                return dp[0, level];
            }

            //题目:100房子，2个鸡蛋，鸡蛋会在某一层摔碎，找出最坏情况的最优解。
            ////理解题意：最坏情况就是尝试次数最大?  最优解 最坏情况的最小尝试次数? 
            ////假设楼层数f=100，鸡蛋个数e=2   

            //int eggs = 2;
            //int floors = 100;
            //int result = SupperEggDrop(eggs, floors);
            //Console.WriteLine("The minimum number of attempts in the worst case is: " + result);

            //Console.ReadLine();


            //基本思路是，对于给定的楼层数 N 和鸡蛋数 K，我们假设在第 X 层楼扔下鸡蛋。
            //如果鸡蛋碎了，我们就需要用剩下的 K-1 个鸡蛋和 X-1 层楼来找出摔碎点；
            //如果鸡蛋没碎，我们就需要用 K 个鸡蛋和 N-X 层楼来找出摔碎点。
            //我们需要取这两种情况下的最大尝试次数，然后加1（加上当前的这次尝试）。

            //我们可以定义一个二维数组 dp[K][N] 来保存这个问题的解，其中 dp[k][n] 表示有 k 个鸡蛋和 n 层楼时所需的最少尝试次数。
            //然后我们可以从底部开始填充这个数组，直到找到 dp[2][100]。

            int SupperEggDrop(int k, int n)
            {
                // 创建一个二维数组来保存解  
                Console.WriteLine($"{k}个鸡蛋，{n}层楼示例:");
                int[,] dp = new int[k + 1, n + 1];

                // 初始化边界条件  
                for (int i = 1; i <= k; i++)
                {
                    dp[i, 0] = 0; // 没有楼层时不需要尝试  
                    if (n > 0)
                    {
                        dp[i, 1] = 1; // 一层楼时只需要尝试一次  
                    }
                }

                for (int j = 1; j <= n; j++)
                {
                    dp[1, j] = j; // 一个鸡蛋时需要尝试最多j次  
                }

                Console.WriteLine($"初始二维数组(0-1层楼尝试次数)后:");
                for (int i = 0; i <= k; i++)
                {
                    for (int j = 0; j <= n; j++)
                    {
                        Console.Write($"{dp[i, j]} ");
                    }
                    Console.WriteLine();
                }



                // 动态规划填充数组  从2个鸡蛋开始
                for (int i = 2; i <= k; i++)
                {
                    for (int j = 2; j <= n; j++)
                    {
                        dp[i, j] = j; // 最坏情况下需要尝试j次  
                        for (int x = 1; x < j; x++)
                        {
                            //如果鸡蛋碎了，我们就需要用剩下的 K-1 个鸡蛋和 X-1 层楼来找出摔碎点；
                            //如果鸡蛋没碎，我们就需要用 K 个鸡蛋和 N-X 层楼来找出摔碎点+1(加上当前这次尝试)
                            dp[i, j] = Math.Min(dp[i, j], Math.Max(dp[i - 1, x - 1], dp[i, j - x]) + 1);
                            // 选择使得最大尝试次数最小的x  
                        }
                    }
                }

                Console.WriteLine($"从2个鸡蛋开始 动态规划填充数组填充数组后:");
                for (int i = 0; i <= k; i++)
                {
                    for (int j = 0; j <= n; j++)
                    {
                        Console.Write($"{dp[i, j]} ");
                    }
                    Console.WriteLine();
                }


                // 返回结果  
                return dp[k, n];
            }


            //二分搜索算法通常用于在已排序的数组中查找特定的元素。然而，对于“鸡蛋掉落”问题，
            //我们需要使用二分搜索的思想来优化尝试次数，但并不能直接使用传统的二分搜索算法。

            //在这个问题中，我们可以使用二分搜索的思想来逼近最优的楼层数，从而最小化在最坏情况下所需的尝试次数。
            //具体做法是，对于当前的尝试次数m和楼层数f，我们可以选择一个中间楼层mid，然后假设在这个楼层扔下鸡蛋：

            //1、如果鸡蛋碎了，那么我们需要在前mid层楼中继续寻找摔碎点，这时我们还有e - 1个鸡蛋和mid层楼。
            //2、如果鸡蛋没碎，那么我们需要在剩下的f - mid层楼中继续寻找摔碎点，这时我们还有k个鸡蛋和f - mid层楼。
            //我们需要找到一个mid，使得上述两种情况下所需的尝试次数最大，但不超过当前的尝试次数m。我们可以从m = 1开始，逐步增加m的值，直到找到一个m，使得我们可以使用e个鸡蛋在f层楼中确定摔碎点。

            int SupperEggDrop0(int e, int f)
            {
                //创建一个二维数组来保存子问题的解
                var dp = new int[e + 1, f + 1];

                //初始化dp数组
                for (int i = 0; i <= e; i++)
                {
                    for (int j = 0; j <= f; j++)
                    {
                        dp[i, j] = int.MaxValue / 2;//使用一个较大的初始值
                    }
                }

                //边界条件
                for (int i = 0; i <= e; i++)
                {
                    dp[i, 0] = 0;//没有楼层时  尝试次数为0
                    if (f > 0)
                    {
                        dp[i, 1] = 1;//只有一层楼时 尝试次数为1
                    }
                }

                Console.WriteLine($"初始二维数组(0-1层楼尝试次数) 且剩余填充最大数:");
                for (int i = 0; i <= e; i++)
                {
                    for (int j = 0; j <= f; j++)
                    {
                        Console.Write($"{dp[i, j]} ");
                    }
                    Console.WriteLine();
                }


                for (int floors = 1; floors <= f; floors++)
                {
                    for (int eggs = 1; eggs <= e; eggs++)
                    {
                        //尝试次数从1开始逐渐增加 
                        for (int attempts = 1; ; attempts++)
                        {
                            int leftMax = 0;//二分法左侧楼层所需最大尝试数
                            int rightMin = int.MaxValue;//二分法右侧楼层所需最小尝试数

                            //找到最优的分割楼层mid
                            for (int mid = 1; mid <= floors; mid++)
                            {
                                leftMax = Math.Max(leftMax, dp[eggs - 1, mid - 1]);// 鸡蛋碎了，下侧楼层所需的最大尝试次数 
                                rightMin = Math.Min(rightMin, dp[eggs, floors - mid] + 1);// 鸡蛋没碎 上侧楼层所需的最小尝试次数加1(当前这次尝试)

                                // 如果下侧楼层所需的最大尝试次数和上侧楼层所需的最小尝试次数之和不超过当前尝试次数  
                                // 那么我们可以减少尝试次数  
                                if (leftMax + rightMin > attempts)
                                    break;
                            }

                            // 找到了最优解，记录到dp数组中  
                            dp[eggs, floors] = attempts;

                            // 如果已经找到了最优解，则退出内层循环  
                            if (leftMax + rightMin == attempts)
                                break;
                        }
                    }
                }

                // 返回结果  
                return dp[e, f];
            }
        }


        #endregion

        #endregion

        #region 配置文件配置加密测试

        {
            #region ConnectionStrings

            var node = "ConnectionStrings";
            Console.WriteLine($"\"{node}\":{{");
            var dbKey = "Default";
            var key = $"{node}:{dbKey}";
            var conn = _configuration[key];
            var encryptStr = EncodingEncryptHelper.Encrypt(conn);
            //var dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
            //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
            //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
            Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\" ");

            dbKey = "Portal";
            key = $"{node}:{dbKey}";
            conn = _configuration[key];
            encryptStr = EncodingEncryptHelper.Encrypt(conn);
            //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
            //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
            //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
            Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\",");

            dbKey = "MySql";
            key = $"{node}:{dbKey}";
            conn = _configuration[key];
            encryptStr = EncodingEncryptHelper.Encrypt(conn);
            //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
            //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
            //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
            Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\",");

            dbKey = "PgSql";
            key = $"{node}:{dbKey}";
            conn = _configuration[key];
            encryptStr = EncodingEncryptHelper.Encrypt(conn);
            //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
            //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
            //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
            Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\",");

            dbKey = "SqlServer";
            key = $"{node}:{dbKey}";
            conn = _configuration[key];
            encryptStr = EncodingEncryptHelper.Encrypt(conn);
            //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
            //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
            //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
            Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\",");

            dbKey = "Write";
            key = $"{node}:{dbKey}";
            conn = _configuration[key];
            encryptStr = EncodingEncryptHelper.Encrypt(conn);
            //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
            //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
            //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
            Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\",");

            dbKey = "Read";
            key = $"{node}:{dbKey}";
            conn = _configuration[key];
            encryptStr = EncodingEncryptHelper.Encrypt(conn);
            //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
            //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
            //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
            Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\" ");
            Console.WriteLine($"}},");


            #endregion

            #region Redis

            node = "Redis";
            Console.WriteLine($"\"{node}\":{{");

            dbKey = "Configuration";
            key = $"{node}:{dbKey}";
            conn = _configuration[key];
            encryptStr = EncodingEncryptHelper.Encrypt(conn);
            //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
            //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
            //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
            Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\",");

            dbKey = "CsRedisConfiguration";
            key = $"{node}:{dbKey}";
            conn = _configuration[key];
            encryptStr = EncodingEncryptHelper.Encrypt(conn);
            //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
            //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
            //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
            Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\",");


            Console.WriteLine($"    \"InstanceName\": \"parakeet\",");
            Console.WriteLine($"    \"ConnectionStrings\": \"localhost\",");
            Console.WriteLine($"    \"DatabaseId\": 1");
            Console.WriteLine($"}},");

            #endregion

            #region AuthServer

            node = "AuthServer";
            Console.WriteLine($"\"{node}\":{{");
            Console.WriteLine($"    \"Authority\": \"https://localhost:50000\",");
            Console.WriteLine($"    \"RequireHttpsMetadata\": true,");
            Console.WriteLine($"    \"ClientId\": \"Parakeet_Web\",");

            dbKey = "ClientSecret";
            key = $"{node}:{dbKey}";
            conn = _configuration[key];
            encryptStr = EncodingEncryptHelper.Encrypt(conn);
            //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
            //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
            //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
            Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\",");

            Console.WriteLine($"    \"IsContainerized\": false");
            Console.WriteLine($"}},");

            #endregion

            #region Settings

            node = "Settings";
            Console.WriteLine($"\"{node}\":{{");

            Console.WriteLine($"      \"DefaultFromAddress\": \"chensq0523@foxmail.com\", //\"chenshuangquan@xywgzs1.onexmail.com\",");
            Console.WriteLine($"      \"DefaultFromDisplayName\": \"Chensq\",");
            Console.WriteLine($"      \"Smtp.Host\": \"smtp.qq.com\", //\"smtp.exmail.qq.com\", //");
            Console.WriteLine($"      \"Smtp.Port\": \"587\", //\"465\", //");
            Console.WriteLine($"      \"Smtp.UserName\": \"chensq0523@foxmail.com\",");

            dbKey = "Smtp.Password";
            key = $"{node}:{dbKey}";
            conn = _configuration[key];
            encryptStr = EncodingEncryptHelper.Encrypt(conn);
            //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
            //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
            //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
            Console.WriteLine($"      \"{dbKey}\": \"{encryptStr}\",");

            Console.WriteLine($"      \"Smtp.EnableSsl\": \"false\", //\"true\",//587端口在emailsender配置中不允许ssl加密  465才可以");
            Console.WriteLine($"      \"Smtp.UseDefaultCredentials\": \"fasle\",");
            Console.WriteLine($"      \"Abp.Mailing.DefaultFromAddress\": \"chensq0523@foxmail.com\",");
            Console.WriteLine($"      \"Abp.Mailing.DefaultFromDisplayName\": \"Chensq\",");
            Console.WriteLine($"      \"Abp.Mailing.Smtp.Host\": \"smtp.qq.com\", //\"smtp.exmail.qq.com\", //");
            Console.WriteLine($"      \"Abp.Mailing.Smtp.Port\": \"587\", //\"465\", //");
            Console.WriteLine($"      \"Abp.Mailing.Smtp.UserName\": \"chensq0523@foxmail.com\", //\"chenshuangquan@xywgzs1.onexmail.com\",");
            Console.WriteLine($"      \"Abp.Mailing.{dbKey}\": \"{encryptStr}\",");
            Console.WriteLine($"      \"Abp.Mailing.Smtp.EnableSsl\": \"false\", //\"true\",//587端口在emailsender配置中不允许ssl加密  465才可以");
            Console.WriteLine($"      \"Abp.Mailing.Smtp.UseDefaultCredentials\": \"false\"");

            Console.WriteLine($"}},");

            #endregion
        }

        #endregion

        #region Guid.Parse 有序Guid生成 测试

        {
            //var guidOne = _guidGenerator.Create();
            //var guidTwo = _guidGenerator.Create();
            //var guidThree = _guidGenerator.Create();

            //Console.WriteLine($" guidOne:{guidOne}\r\n guidTwo:{guidTwo}\r\n guidThree:{guidThree}");
            //Console.ReadKey();

            //////Console.WriteLine($"{Guid.Parse("e9f8e91180e941759adf1a85944ada50")}");//Guid.Parse 可以添加上短横线
            //var option = new AbpSequentialGuidGeneratorOptions
            //{
            //    //DefaultSequentialGuidType = SequentialGuidType.SequentialAtEnd//sqlserver=>SequentialAtEnd
            //    //DefaultSequentialGuidType = SequentialGuidType.SequentialAsBinary
            //    DefaultSequentialGuidType = SequentialGuidType.SequentialAsString//mysql=>SequentialAsString
            //};
            //var optionWarpper = new OptionsWrapper<AbpSequentialGuidGeneratorOptions>(option);
            //////由timeStamp二进制转换的一定时间顺序的guid 够用约5900年，满足大部分项目
            //var sequentialGuidGenerator = new SequentialGuidGenerator(optionWarpper);
            ////var sequenceGuidNext1 = sequentialGuidGenerator.Create();
            ////var sequenceGuidNext2 = sequentialGuidGenerator.Create();
            ////var guid = SimpleGuidGenerator.Instance.Create();//=>等同于Guid.NewGuid();
            ////Console.WriteLine($"sequenceGuidNext1:{sequenceGuidNext1}\nsequenceGuidNext2:{sequenceGuidNext2}\nsimpleGuid:{guid}");
            ////Console.ReadKey();

            ////var sqType = WorkType.设计;
            //Console.WriteLine($"打印SequentialGuidType枚举字符串");
            //Console.WriteLine($"{SequentialGuidType.SequentialAsString}");
            //Console.WriteLine($"{SequentialGuidType.SequentialAsBinary}");
            //Console.WriteLine($"{SequentialGuidType.SequentialAtEnd}");
            ////Console.WriteLine($"sqType:{sqType}");
            ////Console.ReadKey();
        }
        #endregion

        #region Json测试

        {
            {

                ////JsonConvert.SerializeObject()
                //var obj1 = new
                //{
                //    Id = Guid.NewGuid(),
                //    Name = "测试1"
                //};
                //var obj2 = new
                //{
                //    Id = Guid.NewGuid(),
                //    Name = "测试2"
                //};
                //var obj3 = new
                //{
                //    Id = Guid.NewGuid(),
                //    Name = "测试3"
                //};
                //var list = new List<object>
                //{
                //    obj1,obj2,obj3
                //};
                //var userId = Guid.NewGuid();
                //var responsiible = new[]{new
                //{
                //    Id=userId,
                //    Name=new
                //    {
                //        CN="A-质量管理总监",
                //        EN="Quality Director"
                //    },
                //    UserId=userId,
                //    UserName=new
                //    {
                //        CN="A-质量管理总监",
                //        EN="Quality Director"
                //    },
                //    Account="A02"
                //}};
                //var jobject = new JObject
                //{
                //    { $"Id", JToken.FromObject(Guid.NewGuid()) },
                //    { $"ObjectName", JToken.FromObject($"objectName") },
                //    { $"Responsible", JToken.FromObject(responsiible) }
                //};
                //var multiProp = $"MultiProp";
                //jobject.Add(new JProperty($"{multiProp}", JToken.FromObject(new List<object> { obj1 })));

                //if (jobject.ContainsKey(multiProp))
                //{
                //    var refFieldJsonObj = jobject[$"{multiProp}"];
                //    var refFieldArray = JArray.FromObject(refFieldJsonObj);
                //    refFieldArray?.Add(JToken.FromObject(obj2));
                //    jobject[$"{multiProp}"] = JToken.FromObject(refFieldArray);
                //}

                //var userArray = JArray.FromObject(jobject["Responsible"]);
                //var userIds=userArray.Select(x => x.SelectToken("Id").ToString());


                //Console.ReadKey();
            }

            ////dataList 与目标对象少字段测试 success  目标实体多余字段会使用默认值
            //var dataList = new List<WorkflowConfigNodeApprovalActionDto>
            //{
            //    new WorkflowConfigNodeApprovalActionDto
            //    {
            //        Id = sequentialGuidGenerator.Create(),
            //        ActionCode = $"ActionCode1",
            //        BasicInfoId = sequentialGuidGenerator.Create(),
            //        NodeId = sequentialGuidGenerator.Create()
            //    }
            //};

            //var result = JToken.FromObject(dataList).ToObject<List<WorkflowConfigNodeApprovalActionConfig>>();
            ////var result2 = (WorkflowConfigNodeApprovalActionConfig)(dataList.First());

            //var guidJson = $"\"974793a0-6067-751d-017d-3a0a3d59a0e4\"";
            //var guid = JsonConvert.DeserializeObject<Guid>(guidJson);


            //Console.ReadKey();

            //循环查找目录里的json文件，读取并过滤其内容
            var directoryPath = Directory.GetCurrentDirectory();
            foreach (var filePath in Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories))
            {
                var jsonContent = File.ReadAllText(filePath);
                //var jsonObject = JObject.Parse(jsonContent);
                //var obj = JsonConvert.DeserializeObject<object>(jsonContent);
                var searchStr = "特征字符串";
                var matchedTokens = new List<JToken>();
                using (JsonTextReader reader = new JsonTextReader(new StringReader(jsonContent)))
                {
                    while (reader.Read())
                    {
                        if (reader.Value != null && reader.Value.ToString().Contains(searchStr))
                        {
                            JToken token = JToken.ReadFrom(reader);
                            matchedTokens.Add(token);
                            Console.WriteLine("匹配json:");
                            Console.WriteLine(token.ToString());
                            //break;
                        }
                    }
                }
            }

            void FindAndAddMatchedObjects(JToken jsonToken, string searchStr, List<JToken> matchedObjects)
            {
                switch (jsonToken.Type)
                {
                    case JTokenType.Object:
                        foreach (var child in jsonToken.Children<JProperty>())
                        {
                            FindAndAddMatchedObjects(child.Value, searchStr, matchedObjects);
                        }
                        break;
                    case JTokenType.Array:
                        foreach (var item in jsonToken.Children())
                        {
                            FindAndAddMatchedObjects(item, searchStr, matchedObjects);
                        }
                        break;
                    case JTokenType.String when jsonToken.ToString().Contains(searchStr):
                        matchedObjects.Add(jsonToken);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region ServiceProvider IServiceScopeFactory 服务生命周期测试 IServiceScopeFactory为单例注册

        {
            var serviceCollection = new ServiceCollection();
            var root = serviceCollection
                .AddTransient<IFoo, Foo>()
                .AddScoped<IBar, Bar>()
                //.AddSingleton<IBaz, Baz>()
                .BuildServiceProvider();
            serviceCollection.AddSingleton<IBaz, Baz>();

            //serviceCollection.AddTransient<IServiceProviderFactory<ContainerBuilder>, AbpAutofacServiceProviderFactory>();

            //root = serviceCollection.BuildServiceProvider();//默认参数为ServiceProviderOptions.Default
            root = serviceCollection.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,//验证作用域服务未从根容器ServiceProvider提供程序解析
                ValidateOnBuild = true//build时就会检查注册的服务是否能够正常创建
            });
            //猜测abp controller 的依赖注入 autofac应该是改了ValidateScopes 这个东东

            //new AutofacServiceProviderFactory()
            //autofac的提供器 查查源码
            //var root2=serviceCollection.BuildAutofacServiceProvider();

            var serviceScopeFactory1 = root.GetService<IServiceScopeFactory>();
            var serviceScopeFactory2 = root.GetService<IServiceScopeFactory>();
            IServiceProvider child1 = serviceScopeFactory1.CreateScope().ServiceProvider;
            IServiceProvider child2 = serviceScopeFactory1.CreateScope().ServiceProvider;
            IServiceProvider child3 = serviceScopeFactory2.CreateScope().ServiceProvider;
            var serviceScopeFactory3 = child3.GetService<IServiceScopeFactory>();

            Console.WriteLine("ReferenceEquals(root.GetService<IFoo>(), root.GetService<IFoo>() = {0}", ReferenceEquals(root.GetService<IFoo>(), root.GetService<IFoo>()));
            Console.WriteLine("ReferenceEquals(child1.GetService<IBar>(), child1.GetService<IBar>() = {0}", ReferenceEquals(child1.GetService<IBar>(), child1.GetService<IBar>()));
            Console.WriteLine("ReferenceEquals(child1.GetService<IBar>(), child2.GetService<IBar>() = {0}", ReferenceEquals(child1.GetService<IBar>(), child2.GetService<IBar>()));
            Console.WriteLine("ReferenceEquals(child1.GetService<IBaz>(), child2.GetService<IBaz>() = {0}", ReferenceEquals(child1.GetService<IBaz>(), child2.GetService<IBaz>()));
            Console.WriteLine($"child1==child2={ReferenceEquals(child1, child2)}");
            Console.WriteLine($"root==child1={ReferenceEquals(root, child1)}");
            Console.WriteLine($"serviceScopeFactory1==serviceScopeFactory2={ReferenceEquals(serviceScopeFactory1, serviceScopeFactory2)}");
            Console.WriteLine($"serviceScopeFactory2==serviceScopeFactory3={ReferenceEquals(serviceScopeFactory2, serviceScopeFactory3)}");
            //Console.ReadLine();
        }
        #endregion

        #region Ip测试

        {
            //Console.WriteLine($"[DFS:MongoDbConnection]：{configuration["DFS:MongoDbConnection"]}");
            //var testRand = NumberExtensions.GenerateRandomString(2);
            //Console.WriteLine($"随机字符串:{testRand}");
            Console.WriteLine($"GetIp:{LocalIpHelper.GetIp()}");
            Console.WriteLine($"GetLocalIpv4:{LocalIpHelper.GetLocalIpv4()}");
            Console.WriteLine($"GetLocalIpv6:{LocalIpHelper.GetLocalIpv6()}");
        }

        #endregion

        #region 输出颜色测试

        {
            //Log.Logger.Information($"Logger消息颜色测试 CustomMath {Thread.CurrentThread.ManagedThreadId}");
            //Console.BackgroundColor = ConsoleColor.Blue; //设置背景色
            Console.ForegroundColor = ConsoleColor.Red; //设置前景色，即字体颜色
            Console.WriteLine($"Program启动当前线程 CurrentThreadId {Thread.CurrentThread.ManagedThreadId}");
            Console.ResetColor(); //将控制台的前景色和背景色设为默认值

            //Console.WriteLine($"消息颜色测试 CurrentThreadId {Thread.CurrentThread.ManagedThreadId}");

        }

        #endregion

        #region 结构体

        {
            //TestStruct test = default;
            //test.Count = 1000;
            //test.Hours = 100;
            //test.Name = $"测试结构体内存分配";
            ////普通值类型 testInt=5;
            //int testInt = 5;
            //Console.WriteLine($"{testInt}");
            ////int? 类型内存空间分配  struct 应该是在栈中存放  栈中的值到底是个null的值，还是存放的是堆中null的引用地址？
            //int? testInt5 = 20;//反编译看一下
            //int? testIntN = null;//反编译看一下
            //Console.WriteLine($"{testIntN}");
        }

        #endregion

        #region join查询

        {
            //IQurable Join  Linq->Sql Inner Join与Left Join查询
            //var query1 = await _workflowBasicConfigRepository.GetQueryableAsync();
            //var query2 = await _workflowVariableConfigRepository.GetQueryableAsync();
            //query1.Join(query2, s1 => s1.Id, s2 => s2.WorkflowConfigId, (s1, s2) =>new
            //{
            //    Name=s1.Name,
            //    Value=s2.WorkflowConfigId
            //}).DefaultIfEmpty();//右联接查询 把query1与query2调换位置
        }

        #endregion

        #region 并行任务Parallel-> ParallelLoopResult

        {
            ////Parallel.For()和Paraller.ForEach()方法在每次迭代中调用相同的代码，而Parallel.Invoke()方法允许同时调用不同的方法。
            ////Parallel.ForEach()用于数据并行性，Parallel.Invoke()用于任务并行性；

            //var result1 = Parallel.For(0, 10, (i, state) =>
            //{
            //    Console.WriteLine("迭代次数：{0},任务ID:{1},线程ID:{2}", i, Task.CurrentId, Thread.CurrentThread.ManagedThreadId);
            //    Thread.Sleep(10);
            //    if (i > 5)
            //        state.Break();
            //});
            //var data = new List<string>{ "str1", "str2", "str3", "str4", "str5" };
            //ParallelLoopResult result = Parallel.ForEach<string>(data, (str, state, i) =>
            //{
            //    Console.WriteLine("迭代次数：{0},{1}", i, str);
            //    if (i > 3)
            //    {
            //        state.Break();
            //    }
            //});
            //Console.WriteLine("是否完成:{0}", result.IsCompleted);
            //Console.WriteLine("最低迭代:{0}", result.LowestBreakIteration);

            //Parallel.Invoke(() =>
            //{
            //    Thread.Sleep(100);
            //    Console.WriteLine("method1");
            //}, () =>
            //{
            //    Thread.Sleep(10);
            //    Console.WriteLine("method2");
            //});

        }

        #endregion

        #region 测试take数量大于集合数量 集合数组

        {
            ////测试take数量大于集合数量
            //var intList = new List<int>();//Enumerable.Range(1, 30).ToList();
            //var ints = intList.Skip(0).Take(50).ToList();

            //Console.ReadKey();

            //var list = new List<string>();
            //Console.WriteLine($"new list 空集合允许 string.Join string.Join(\",\", list).HasValue():{string.Join(",", list).HasValue()}");
            //Console.WriteLine($"new list 空集合允许 string.Join string.Join(\",\", list) == null :{string.Join(",", list) == null}");
            //Console.WriteLine($"new list 空集合允许 string.Join string.Join(\",\", list) == string.Empty :{string.Join(",", list) == string.Empty}");
            //Console.ReadKey();

            //var result = new ResponseWrapper();
            //result.Message = "1";
            //result.Messages.Add(result.Message);
            //result.Message = "2";
            //result.Messages.Add(result.Message);
            //result.Message = "3";
            //result.Messages.Add(result.Message);
            //result.Message = "4";
            //result.Messages.Add(result.Message);
            //result.Message = "5";
            //result.Messages.Add(result.Message);
            //Console.WriteLine($"join:{string.Join(",",result.Messages)}");
            //Console.ReadKey();

        }

        #endregion

        #region DateTime  计算测试

        {
            //DateTime? fromTime = new DateTime(2021, 5, 28,12,0,0);
            //DateTime? toTime = null;//new DateTime(2021, 5, 28, 18, 0, 0);
            //var now = DateTime.Now;//new DateTime(2021,5,29);//查看2月最后一天呢，5月31 5月30 5月29 5月30 三个月前都是2月28日
            //fromTime ??= (toTime ??= now).AddMonths(-3);
            //toTime ??= fromTime.Value.AddMonths(3);
            //var period = (toTime - fromTime).Value.TotalDays;

            //var computeTime = toTime?.AddDays(1);
            //Console.WriteLine($"{computeTime}");
            DateTime.Now.ToUnixTimeTicks();
        }

        #endregion

        #region null?计算优先级 及字string.join 集合测试

        {
            ////继承Object ValueTypeNullable<T>
            ////struct Nullable<T> where T : struct  可空类型始终是结构体，属于值类型。存储方式(堆/栈)根据值类型所处的位置而不同
            ////{
            ////    bool hasValue;
            ////    T value;
            ////}
            //bool? testBool = true;//外部定义的值类型都是存放到栈上(栈上的值为true)
            //int? testInt = null;//外部定义的值类型都是存放到栈上(栈上的值为null)
            ////People本来就是Class引用类型，因为没有new就没有分配堆内存，所以此处仅仅生成了一个空指针(栈上值为null)，
            ////如果new一个people指针会指向堆内存，Peopeo内的所有值类型均存放在堆内存里
            //People? people = null;

            ////bool *testBoolz = &testBool; 指针的用法?

            //Console.WriteLine($"typeof(int?):{typeof(int?)}");
            //Console.WriteLine($"typeof(int?).IsValueType:{typeof(int?).IsValueType}");
            //Console.WriteLine($"typeof(bool?):{typeof(bool?)}");
            //Console.WriteLine($"typeof(bool?).IsValueType:{typeof(bool?).IsValueType}");
            //Console.WriteLine($"testBool.GetType().IsValueType:{testBool.GetType().IsValueType}");
            //Console.WriteLine($"testInt?.GetType().IsValueType:{testInt?.GetType().IsValueType}");

            //Console.WriteLine($"people?.GetType().IsValueType:{people?.GetType().IsValueType}");


            //decimal? price = null;
            //decimal? amount = null;
            //decimal? total = price * amount ?? 0;
            //Console.WriteLine($"price ={price}");
            //Console.WriteLine($"amount ={amount}");
            //Console.WriteLine($"price * amount ?? 0={price * amount ?? 0}");
            //Console.WriteLine($"(price * amount) ?? 0={(price * amount) ?? 0}");
            //Console.WriteLine($"price * (amount ?? 0)={price * (amount ?? 0)}");
            //Console.ReadKey();

            //var array = new List<string>();

            //Console.WriteLine($"array.Any()： {array.Any()}");
            //Console.WriteLine($"array.Count： {array.Count()}");
            //Console.WriteLine($"array join： {string.Join(",", array)}");

            //var test = string.Join(",", array);
            //Console.WriteLine($"test： {test}");
            //Console.WriteLine($"test is null： {test is null}");

            //Console.WriteLine($"test.HasValue()： {test.HasValue()}");
            //Console.WriteLine($"string.IsNullOrEmpty(test)： {string.IsNullOrEmpty(test)}");
            //Console.WriteLine($"string.IsNullOrWhiteSpace(test)： {string.IsNullOrWhiteSpace(test)}");

            //var array1 = test.Split(',');//test.Split(",");

            //Console.WriteLine($"array1.Any()： {array1.Any()}");
            //Console.WriteLine($"array1.Count： {array1.Count()}");
            //Console.WriteLine($"array1 join： {string.Join(",", array1)}");
            //Console.ReadKey();
        }

        #endregion

        #region 进制转换

        {
            //var show = "1测试一下十六进制大小写展示9";
            //var bytes=Encoding.UTF8.GetBytes(show);
            //foreach (var b in bytes)
            //{
            //    Console.WriteLine($"X2:{b:X2}");
            //    Console.WriteLine($"X:{b:X}");
            //    Console.WriteLine($"x:{b:x}");
            //    Console.WriteLine($"x2:{b:x}");
            //}
            //Console.ReadKey();

            //ConverterTest.HexToDec();
            //Console.ReadKey();
        }

        #endregion

        #region 数据类型转换

        {
            //es 数据类型转换
            //var typeList = "string,text,keyword,integer,long,short,int,double,float,half_float,scaled_float,bool,boolean,date,range,byte,binary,array,object,nested,geo_point,geo_shape,ip,completion,token_count,attachment,percolator"
            //    .Split(",").ToList();

            //foreach (var typeString in typeList)
            //{
            //    var gType = ReflectTypeHelper.GetTypeByEsTypeString(typeString);
            //    Console.WriteLine($"{typeString}:{gType}");
            //}
            //Console.ReadKey();

            //long tickes = DateTime.Now.Ticks;
            //var test = (int)tickes;  // 10000000;
            //Console.WriteLine($"DateTime.Now.Ticks：{tickes}_test:{test}");
            //Console.ReadKey();

            //var switchMessage = Convert.ToBoolean("true");//false

            //Console.WriteLine($"字符串false转bool：{switchMessage}");
            //Console.ReadKey();
        }

        #endregion

        #region sql注入正则验证

        {

            ////匹配次数
            ////var testRegex1 = new Regex(@"[+|-|*|/|%|>|<|>=|<=|!=|&|&&|\|\|]", RegexOptions.IgnoreCase);
            //var testString1 = "1+2+3-4*5/6%7||3+2-4*5/6";
            //Console.WriteLine($"testString1： {testString1}");
            //var items = Regex.Matches(testString1, @"\+|-|\*|/|%|>|<|<=|>=|!=|&{1,2}|\|{2}", RegexOptions.IgnoreCase);
            ////var items=Regex.Matches(testString1,"[+|\\-|*|/|%|>|<|>=|<=|!=|&|&&|(\\|\\|)]", RegexOptions.IgnoreCase);
            //Console.WriteLine($"testString1正则校验次数： {items?.Count}");
            //Console.ReadKey();


            ////yyyy-MM-dd
            //var testRegex = new Regex(@"from indexname", RegexOptions.IgnoreCase);
            //var testString = "select count(*) from indexName where 1=1";
            //Console.WriteLine($"testString： {testString}");
            //Console.WriteLine($"testString正则校验： {testRegex.IsMatch(testString)}");
            //Console.ReadKey();
            ////var parameters = "InstanceSort,BizTitle,StatusId,StatusName.Cn,Create.Time";//.Split(",");
            ////var condition = $"StatusId!='1c50b5df-f925-4f67-a276-b1738aeb9a7a'";

            ////var valid = SqlHelper.IsValidInput(parameters);
            ////valid = SqlHelper.IsValidInput(condition);

            ////Console.ReadKey();
        }

        #endregion

        #region 静态类测试

        {
            //Console.WriteLine($"静态类构造函数现在是第一次使用时构造 测试静态类:TypeExtensions");
            //var baseType = typeof(BaseEntity<>);
            //var children = baseType.Assembly.GetTypes()
            //    .Where(x => x.BaseType?.IsGenericType == true && x.IsImplementedBaseType(baseType))
            //    .ToList();
            //Console.ReadKey();
        }

        #endregion

        #region Policy测试

        {

            //重试多次，在每次重试时调用一个操作
            //对于当前的异常，重试计数和上下文
            //提供执行()
            //var policy = Policy
            //        .Handle<DivideByZeroException>()
            //        .Retry(3, onRetry: (exception, retryCount, context) =>
            //        {
            //            //在每次重试(如日志记录)之前添加要执行的逻辑
            //            throw new Exception($"第{retryCount}次重试");
            //        }); 
            //policy.Execute(() =>
            //{
            //    //模拟错误
            //    var number = 1;
            //    var error = number / 0;
            //});

            //try
            //{
            //    Console.WriteLine($"看看我try return什么时候执行:{DateTime.Now}");
            //    return await Task.FromResult(1);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    throw;
            //}
            //finally
            //{
            //    Console.WriteLine($"看看我finally什么时候执行:{DateTime.Now}");
            //}
            ////Console.ReadKey();
        }

        #endregion

        #region 测试

        {
            #region 配置文件读取数组
            //Console.WriteLine("Hello World!");
            //Console.WriteLine($"{typeof(CustomConfigurationManager).Assembly.GetName().Name}");
            //Console.WriteLine($"{typeof(CustomConfigurationManager).Assembly.GetName()}");
            //Console.WriteLine($"NetCoreDomainModule所在路径:{Path.GetDirectoryName(typeof(NetCoreDomainModule).Assembly.Location)}");
            //Console.WriteLine($"NetCoreApplicationModule所在路径:{Path.GetDirectoryName(typeof(NetCoreApplicationModule).Assembly.Location)}");
            //Console.WriteLine($"NetCoreEntityFrameworkCoreModule所在路径:{Path.GetDirectoryName(typeof(NetCoreEntityFrameworkCoreModule).Assembly.Location)}");
            //Console.WriteLine($"Program所在路径:{Path.GetDirectoryName(typeof(Program).Assembly.Location)}");

            //var section = CustomConfigurationManager.Configuration.GetSection("App").GetSection("Email");
            //Console.WriteLine($"{section.GetSection("SmtpHost")?.Value}");

            //var writeArray = CustomConfigurationManager.Configuration.GetSection("ConnectionItems")
            //    .GetSection("Write").GetChildren().Select(m => m.Value).ToArray();
            //var readArray = CustomConfigurationManager.Configuration.GetSection("ConnectionItems")
            //    .GetSection("Read").GetChildren().Select(m => m.Value).ToArray();
            //Console.WriteLine($"writeArray.Length:{writeArray.Length}，readArray.Length:{readArray.Length}");
            #endregion

            #region 数据类型测试与转换
            //var guidNew = Guid.NewGuid().ToString();
            //var guid=(Guid)Convert.ChangeType(Guid.Parse(guidNew), typeof(Guid));
            //Console.WriteLine($"{guidNew}转换结果:{guid}");
            //var intNew = 145.ToString();
            //var intp=(int)Convert.ChangeType(intNew, typeof(int));
            //Console.WriteLine($"{intNew}转换结果:{intp}");

            //var intShow = (default(int?) ?? 0).ToString();
            //Console.WriteLine($"int?.ToString():{intShow}");
            //var decimalShow = default(decimal?)?.ToString();
            //Console.WriteLine($"decimal?.ToString():{decimalShow}");

            #region C#语法基础
            ////boolean 布尔型                 1 / 8
            ////byte 字节类型                    1
            ////char 字符型                       2  一个字符能存储一个中文汉字
            ////short 短整型                     2
            ////int 整数类型                      4
            ////float 浮点类型（单精度）  4
            ////long 长整形                      8
            ////double 双精度类型（双精度）  8
            //一、整形
            //    int占据操作系统一个内存单元的大小。long跟int相同
            //    早先16位操作系统一个内存单元是16位，所以是2个字节；32位系统一个内存单元是是32位，所以是4字节；64位操作系统一个内存单元是16位，故占8个字节。

            //二、字符型
            //    char类型通常占据一个字节，对于用于扩展字符集的wchar_t类型，需要占据两个字节。
            //三、布尔型

            //    bool占据一个字节
            //四、浮点型
            //    float占据4个字节，double是float的两倍即8个字节
            //五、指针
            //    指针字节长度计算原理其实跟int差不多，一个指针的位数和操作系统的位数是相等的，即32位系统应该是4个字节，64位系统应该是8个字节。
            //bool -> System.Boolean(布尔型，一个bit位(0/1),其值为 true(1) 或者 false(0))

            //byte -> System.Byte(字节型，占 1 字节，表示 8 位正整数，范围 0 ~255)

            //sbyte -> System.SByte(带符号字节型，占 1 字节，表示 8 位整数，范围 - 128 ~127)

            //char -> System.Char(字符型，占有两个字节，表示 1 个 Unicode 字符)

            //short -> System.Int16(短整型，占 2 字节，表示 16 位整数，范围 - 32, 768 ~32, 767)

            //ushort -> System.UInt16(无符号短整型，占 2 字节，表示 16 位正整数，范围 0 ~65, 535)

            //uint -> System.UInt32(无符号整型，占 4 字节，表示 32 位正整数，范围 0 ~4, 294, 967, 295)

            //int -> System.Int32(整型，占 4 字节，表示 32 位整数，范围 - 2, 147, 483, 648 到 2, 147, 483, 647)

            //float -> System.Single(单精度浮点型，占 4 个字节)

            //ulong -> System.UInt64(无符号长整型，占 8 字节，表示 64 位正整数，范围 0 ~大约 10 的 20 次方)

            //long -> System.Int64(长整型，占 8 字节，表示 64 位整数，范围大约 - (10 的 19) 次方 到 10 的 19 次方)

            //double -> System.Double(双精度浮点型，占8 个字节)


            ////1、数据类型 byte short int long float decimal
            //byte b = 255;//-2^7～(2^7-1) 最大255
            //short s = 255;//-2^15～(2^15-1)
            //int i = 2147483647;//-2^31～(2^31-1)
            //long l = 2 ^ 63 - 1;//-2^63～(2^63 -1)
            //float f = 0.4f;//float的范围为-2^128 ~ +2^128（-3.40E+38 ~ +3.40E+38） 
            //double d = 0.6d;//double的范围为-2^1024 ~ +2^1024（-1.79E+308 ~ +1.79E+308）   小数默认为double类型
            ////种类------ - 符号位------------ - 指数位----------------尾数位----
            ////float---第31位(占1bit)-- - 第30 - 23位(占8bit)----第22 - 0位(占23bit)
            ////double--第63位(占1bit)-- - 第62 - 52位(占11bit)-- - 第51 - 0位(占52bit)
            ////取值范围主要看指数部分： 
            ////float的指数部分有8bit(2 ^ 8)，由于是有符号型，所以得到对应的指数范围 -128~128。 
            ////double的指数部分有11bit(2 ^ 11)，由于是有符号型，所以得到对应的指数范围 -1024~1024。 
            ////由于float的指数部分对应的指数范围为 - 128~128，所以取值范围为： 
            ////-2 ^ 128到2 ^ 128，约等于 - 3.4E38 ～ +3.4E38

            ////float：2 ^ 23 = 8388608，一共七位，这意味着最多能有7位有效数字，但绝对能保证的为6位，也即float的精度为6~7位有效数字；
            ////double：2 ^ 52 = 4503599627370496，一共16位，同理，double的精度为15~16位。

            ////浮点意思是小数点浮动, 当小数点浮动到所有数字之后就变成了整数，所以浮点数包含整数与小数 同理double类型包含前面所有类型

            ////因float与double有精度损耗  double类型数据范围比decimal的大，所以不能把double类型数据直接赋值给decimal类型变量
            ////另外：浮点数之间的转换只存在float=>double一种  decimal,double互转或转为其它数字类型都需要强制类型转换。
            //decimal de = 0.6m;//十进制类型 不是基础类型 使用时有性能损耗  精度较高 数字后面加上m 来区别 取值范围-7.9*10^28/(10^(0～28))～7.9*10^28/(10^(0～28))

            #endregion

            #endregion

            #region 文件夹路径

            //var path =
            //    $@"D:\Projects\ParakeetABPCore\aspnet-core\src\Common.Web\wwwroot/upload/388053b5-a674-12d9-a659-39f428d991ec/SAM_1059.png";
            //var directory=new DirectoryInfo(path);
            //if (directory.Parent != null && (directory.Parent.Exists&&directory.Parent.GetFiles().Any()))
            //{
            //    Console.WriteLine(directory.FullName.LastIndexOf(@"\"));
            //    Console.WriteLine(directory.FullName.LastIndexOf(@"/"));
            //}
            #endregion

            #region 十进制转16进制

            //var sequenceNos = Enumerable.Range(1, 20);
            //foreach (var sequenceNo in sequenceNos)
            //{
            //    Console.WriteLine($"{sequenceNo.ToString("x4").PadLeft(4,'0')}");
            //}

            #endregion

            #region 日期格式验证
            //var dateStringRegx = new Regex(@"^(?:(?!0000)[0-9]{4}(?:(?:0[1-9]|1[0-2])-(?:0[1-9]|1[0-9]|2[0-8])|(?:0[13-9]|1[0-2])(?:29|30)|(?:0[13578]|1[02])-31)|(?:[0-9]{2}(?:0[48]|[2468][048]|[13579][26])|(?:0[48]|[2468][048]|[13579][26])00)-02-29)$");
            //var array = $"{DateTime.Now.ToString("yyyyMMdd")}-{DateTime.Now.ToString("yyyyMMdd")}".Split('-');
            //if (array.Length < 2 || (!(dateStringRegx.IsMatch(array.First()) && dateStringRegx.IsMatch(array.Last()))))
            //{
            //    Console.WriteLine($"请输入有效日期格式yyyyMMdd-yyyyMMdd");
            //}

            ////yyyy-MM-dd
            //var dateRegex = new Regex(@"^(?:(?!0000)[0-9]{4}-(?:(?:0[1-9]|1[0-2])-(?:0[1-9]|1[0-9]|2[0-8])|(?:0[13-9]|1[0-2])-(?:29|30)|(?:0[13578]|1[02])-31)|(?:[0-9]{2}(?:0[48]|[2468][048]|[13579][26])|(?:0[48]|[2468][048]|[13579][26])00)-02-29)$");
            //var dateString = DateTime.Now.ToString("yyyy-MM-dd");
            //Console.WriteLine($"yyyyMMdd日期： {dateString}");
            //Console.WriteLine($"dateString正则校验： {dateRegex.IsMatch(dateString)}");

            ////yyyyMM-dd
            //var dateRegex1 = new Regex(@"^(?:(?!0000)[0-9]{4}(?:(?:0[1-9]|1[0-2])-(?:0[1-9]|1[0-9]|2[0-8])|(?:0[13-9]|1[0-2])-(?:29|30)|(?:0[13578]|1[02])-31)|(?:[0-9]{2}(?:0[48]|[2468][048]|[13579][26])|(?:0[48]|[2468][048]|[13579][26])00)-02-29)$");
            //var dateString1 = DateTime.Now.ToString("yyyyMM-dd");
            //Console.WriteLine($"yyyyMM-dd日期： {dateString1}");
            //Console.WriteLine($"dateString1正则校验： {dateRegex1.IsMatch(dateString1)}");

            ////yyyyMMdd
            //var dateRegex2 = new Regex(@"^(?:(?!0000)[0-9]{4}(?:(?:0[1-9]|1[0-2])(?:0[1-9]|1[0-9]|2[0-8])|(?:0[13-9]|1[0-2])-(?:29|30)|(?:0[13578]|1[02])-31)|(?:[0-9]{2}(?:0[48]|[2468][048]|[13579][26])|(?:0[48]|[2468][048]|[13579][26])00)-02-29)$");
            //var dateString2 = DateTime.Now.ToString("yyyyMMdd");
            //Console.WriteLine($"yyyyMMdd日期： {dateString2}");
            //Console.WriteLine($"dateString2正则校验： {dateRegex2.IsMatch(dateString2)}");




            //tickes 测试
            //var now = DateTime.Now;
            //var ts1= (now.Ticks - TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local).Ticks) / 10000 / 1000;
            //var ts2= (now.ToUniversalTime().Ticks - TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local).ToUniversalTime().Ticks) / 10000 / 1000;
            //var ts3= (now.ToUniversalTime().Ticks - TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local).Ticks) / 10000 / 1000;
            //Console.WriteLine($"now.Ticks： {now.Ticks}");
            //Console.WriteLine($"now.ToUniversalTime().Ticks： {now.ToUniversalTime().Ticks}");
            //Console.WriteLine($"ts1： {ts1}");
            //Console.WriteLine($"ts2： {ts2}");
            //Console.WriteLine($"ts3： {ts3}");


            #endregion

            #region 加密信息结果测试

            {
                //var token = SHAHelper.HMACSHA256("appId", "appKey", "appSecret", 1620713178);
                ////Console.WriteLine($"字符串:a=zjlhx&s=iot_mC7CCLTd&t=2021051112&sLetter=9");
                //Console.WriteLine($"字符串:appId:appId");
                //Console.WriteLine($"字符串:a=appId&s=appSecret&t=1620713178&sLetter=9");
                //Console.WriteLine($"token:{token}");

                //生成随机密钥
                //var secret = SHAHelper.SHA1("iot_pipenet2021").ToUpper();
                //Console.WriteLine($"secret:{secret}");
            }

            #endregion
            try
            {
                //Console.WriteLine($"测试httpclient request");

                #region 公共参数
                //var base64Phto = Utilities.Base64Phto;
                //var base64RemovePrefix = base64Phto.RemoveBase64ImagePrefix();
                ////var test = base64RemovePrefix.Split(",").LastOrDefault();//test.KeepBase64ImagePrefix();
                ////Console.WriteLine($"base64RemovePrefix.Equals(test):{base64RemovePrefix.Equals(test)}");
                //var encodePhoto = System.Web.HttpUtility.UrlEncode(base64RemovePrefix);
                var host = "http://www.ruimap.com";
                var port = 8060;
                var apiUrl = "/api/test";
                var postParameter = new PostParameterDto
                {
                    Host = host,
                    Port = port,
                    Api = apiUrl
                };
                #endregion

                #region 四川省厅

                //////加解密测试
                //////Console.WriteLine($"加密测试：{AESEncrypt.Encrypt("50022319568248","","")}");
                //////Console.WriteLine($"加密测试：{AESEncrypt.Decrypt("97KnhUWqsXi9H3JD/WtfXvJfghAm984rxPa5SGM4Oxg=")}");

                //var api_root = "202.61.90.35";//测试接口地址http://202.61.90.35:8010/    http://182.150.28.195:8010/SMZ3
                //host = $"http://{api_root}";
                //port = 8010;
                //postParameter.Host = host;
                //postParameter.Port = port;
                //apiUrl = $"/SMZ3/api/Post/Data";
                //postParameter.Api = apiUrl;//所有实名制接口api都一样

                ////登录token测试
                //postParameter.AccessToken = (await SichuanTest.GetAccessToken())?.Data.Access_token;

                #region 4.1.1	项目基本信息（表名：ProjectInfo）Content

                //postParameter.Content = SichuanTest.GetProjectInfoContent();
                //await ClientPost(postParameter);
                //await Task.Delay(1000);
                #endregion

                #region 4.1.2	项目状态信息（表名：ProjectStatusInfo）Content

                //postParameter.Content = SichuanTest.GetProjectStatusInfoContent();
                //await ClientPost(postParameter);
                //await Task.Delay(1000);
                #endregion

                #region 4.1.3	施工许可证信息（表名：ProjectBuilderLicense）Content

                //postParameter.Content = SichuanTest.GetProjectBuilderLicenseContent();
                //await ClientPost(postParameter);
                //await Task.Delay(1000);
                #endregion


                #region 4.1.4	竣工验收信息（表名：TBProjectFinishCheckInfo）Content

                //postParameter.Content = SichuanTest.GetTBProjectFinishCheckInfoContent();
                //await ClientPost(postParameter);
                //await Task.Delay(1000);
                #endregion

                #region 4.2.1	参建单位信息（表名：ProjectCorpInfo）Content

                //postParameter.Content = SichuanTest.GetProjectCorpInfoContent();
                //await ClientPost(postParameter);
                //await Task.Delay(1000);
                #endregion

                #region 4.2.2	参建单位进出场信息（表名：ProjectCorpInoutInfo） Content

                //postParameter.Content = SichuanTest.GetProjectCorpInoutInfoContent();
                //await ClientPost(postParameter);
                //await Task.Delay(1000);
                #endregion

                #region 4.2.3	管理人员信息（表名：ProjectPMInfo）Content

                //postParameter.Content = SichuanTest.GetProjectPMRegisterContent();
                //await ClientPost(postParameter);
                //await Task.Delay(1000);
                #endregion

                #region 4.3.1	项目班组信息（表名：TeamMasterInfo）Content

                //postParameter.Content = SichuanTest.GetTeamMasterInfoContent();
                //await ClientPost(postParameter);
                //await Task.Delay(1000);
                #endregion

                #region 4.3.2	班组进出场信息（表名：TeamInoutInfo）Content

                //postParameter.Content = SichuanTest.GetTeamInoutInfoContent();
                //await ClientPost(postParameter);
                //await Task.Delay(1000);
                #endregion

                #region 4.3.3	建筑工人信息（表名：ProjectWorkerInfo）Content 建筑工人信息 实名制注册/

                ////postParameter.Api = $"/SMZ3/api/ProjectWorkerInfo/Post/Data";
                //postParameter.Content = SichuanTest.GetProjectWorkerRegisterContent();
                //await ClientPost(postParameter);
                //await Task.Delay(1000);

                //postParameter.Content = SichuanTest.GetProjectPMRegisterContent();
                //await ClientPost(postParameter);
                //await Task.Delay(1000);
                #endregion

                #region 4.3.4	建筑工人进出场信息（表名：ProjectWorkerInoutInfo）建筑工人进出场信息 

                ////apiUrl = "/SMZ3/api/ProjectWorkerInoutInfo/Post/Data";
                ////postParameter.Api = apiUrl;
                //postParameter.Content = SichuanTest.GetProjectWorkerInoutContent();
                ////postParameter.AccessToken = (await SichuanTest.GetAccessToken()).Access_token;
                //await ClientPost(postParameter);
                #endregion

                #region 4.3.5	劳动合同信息（表名：WorkerContractInfo）

                //postParameter.Content = SichuanTest.GetWorkerContractInfoContent();
                //await ClientPost(postParameter);
                //await Task.Delay(1000);
                #endregion

                #region 4.3.6	考勤信息（表名：WorkerAttendanceInfo）

                ////apiUrl = "/SMZ3/api/WorkerAttendanceInfo/Post/Data";
                ////postParameter.Api = apiUrl;
                //postParameter.Content = SichuanTest.GetWorkerAttendanceContent();
                ////postParameter.AccessToken = (await SichuanTest.GetAccessToken()).Access_token;
                //await ClientPost(postParameter);
                //await Task.Delay(1000);
                #endregion



                #region 扬尘设备信息表(表名：DustDeviceInfo)

                ////扬尘 需要找地方要测试地址
                ////api_root = "202.61.90.35";//测试接口地址http://202.61.90.35:8010/    http://182.150.28.195:8010/SMZ3
                ////host = $"http://{api_root}";
                ////port = 8010;
                ////postParameter.Host = host;
                ////postParameter.Port = port;

                ////apiUrl = "/DustDeviceInfo/Post/Data";
                ////postParameter.Api = apiUrl;
                //postParameter.Content = SichuanTest.GetDustDeviceContent();
                ////postParameter.AccessToken = (await SichuanTest.GetAccessToken()).Access_token;
                //await ClientPost(postParameter);
                //await Task.Delay(1000);
                #endregion

                #region 扬尘监测数据表(表名：DustMonitorInfo)

                ////apiUrl = "/DustMonitorInfo/Post/Data";
                //postParameter.Api = apiUrl;
                //postParameter.Content = SichuanTest.GetDustMonitorContent();
                ////postParameter.AccessToken = (await SichuanTest.GetAccessToken()).Access_token;
                //await ClientPost(postParameter);
                //await Task.Delay(1000);
                #endregion

                #region 扬尘超标数据表(表名：DustExcessiveInfo)

                ////apiUrl = "/DustExcessiveInfo/Post/Data";
                //postParameter.Api = apiUrl;
                //postParameter.Content = SichuanTest.GetDustExcessiveContent();
                ////postParameter.AccessToken = (await SichuanTest.GetAccessToken()).Access_token;
                //await ClientPost(postParameter);
                //await Task.Delay(1000);
                #endregion

                #region 扬尘超标视频叠加参数报警图片(表名：DustExcessiveImg)

                ////apiUrl = "/DustExcessiveImg/Post/Data";
                //postParameter.Api = apiUrl;
                //postParameter.Content = SichuanTest.GetDustExcessiveImgContent();
                ////postParameter.AccessToken = (await SichuanTest.GetAccessToken()).Access_token;
                //await ClientPost(postParameter);
                //await Task.Delay(1000);
                #endregion

                #endregion

                #region chengdu

                //host = "http://118.122.92.139";//"http://pt.cdzj.chengdu.gov.cn";//
                //port = 6965;

                #region 实名制注册

                //port = 6961;//8066;//
                //apiUrl = "/Regist.Asfx";
                //postParameter.Host = host;
                //postParameter.Port = port;
                //postParameter.Api = apiUrl;
                //postParameter.Content = ChengduTest.GetRegisterContentByFormData();//ChengduTest.GetRegisterContentByFormData();//
                //await ClientPost(postParameter);
                #endregion

                #region 闸机通行

                //host = "http://pt.cdzj.chengdu.gov.cn";//"http://118.122.92.139";//
                //port = 8065;//6965
                //apiUrl = "/Service/DevivePacketWebSvr.assx/UploadAttendance";
                //var content = ChengduTest.GetGateContent();//ChengduTest.GetRegisterContent();//

                #endregion

                #region 操作反馈

                //host = "http://pt.cdzj.chengdu.gov.cn";//"http://118.122.92.139";//
                //port = 8065;//6965
                //apiUrl = "/Service/DevivePacketWebSvr.assx/FeedBack";
                //var content = ChengduTest.GetFeedBackContent();//ChengduTest.GetRegisterContent();//

                #endregion

                #region 塔吊运行数据

                //host = "101.207.139.194";//"http://test.lemcd.tower.com";//测试环境 ping不通
                //port = 9999;
                //host = "http://118.122.92.139";//"http://test.lemcd.tower.com";//测试环境
                //port = 6965;
                //apiUrl = "/api/dev/tower/tw-rundata";
                ////apiUrl = "/api/dev/tower/tw-workcycle";
                ////apiUrl = "/api/dev/tower/tw-heart-beat";
                //var content = ChengduTest.GetCraneRunContent();

                #endregion

                #endregion

                #region Cdceg

                #region 扬尘 OK api:/jgapi/dust/upload
                //apiUrl = "/jgapi/dust/upload";
                //var content = await CdcegTest.GetEnvironmentContent();
                #endregion

                #region 塔机 OK  api:/jgapi/tower/upload
                //apiUrl = "/jgapi/tower/upload";
                //var content = await CdcegTest.GetCraneContent();
                #endregion

                #region Gate闸机考勤数据上传：  /jgapi/attendance/jwuprecdatarelay
                //apiUrl = "/jgapi/attendance/jwuprecdatarelay";


                #endregion

                #region 实名制注册    api:/jgapi/attendance/regist
                //apiUrl = "/jgapi/attendance/regist";

                //var content = await CdcegTest.GetRegisterContent();

                #endregion

                #region 升降机

                //var lifter = new
                //{
                //    sourceId = "a38611f8-ff08-40b7-95a1-f276df0f866e",
                //    serialNo = "TSVS837819829",
                //    showName = "项目监控",
                //    camNo = 1,
                //    coverUrl = "http://jwfs.typeo.org/a38611f8ff0840b795a1f276df0f866e.png",
                //    getUrl = "http://jwfs.typeo.org/api/v1/video/a38611f8-ff08-40b7-95a1-f276df0f866e",
                //    playUrl = "http://hls01open.ys7.com/openlive/a38611f8ff0840b795a1f276df0f866e.m3u8"
                //};
                #endregion

                #endregion

                #region Hunan

                //host = "http://39.108.81.42";
                //port = 9080;
                ////var projectKey = "5b73796d-7f86-46e3-b743-3d15dfe1ea86";
                ////var token = "48efe90b03b947abae25a3ddad170af4";
                //postParameter.Host = host;
                //postParameter.Port = port;

                ////#region 扬尘
                //postParameter.Api = "/open/api/reportDust";//扬尘
                //postParameter.Content = HunanTest.GetEnvironmentContent();
                //await ClientPost(postParameter);
                //#endregion

                //#region 噪声
                //apiUrl = "/open/api/reportDb";//噪声
                //var content = HunanTest.GetNoiseContent();
                //#endregion

                //#region 考勤
                //apiUrl = "/open/api/reportStaffPunch";//考勤
                //var content = HunanTest.GetGateContent();
                //#endregion

                //#region 塔吊基础信息上传
                //postParameter.Api = "/open/api/reportDeviceTdBase";//塔吊基础信息
                //postParameter.Content = HunanTest.GetCraneBasicContent();
                //await ClientPost(postParameter);

                //Console.ReadKey();
                //#endregion

                //#region 塔吊

                //apiUrl = "/open/api/reportTD";//塔吊
                //var content = HunanTest.GetCraneContent();
                //#endregion

                //#region 升降机基础数据上传

                //apiUrl = "/open/api/reportDeviceSjjBase";//升降机基础信息
                //var content = HunanTest.GetLifterBasicContent();
                //#endregion

                //#region 升降机数据上传

                //apiUrl = "/open/api/reportDeviceSjj";//升降机
                //var content = HunanTest.GetLifterContent();
                //#endregion

                //#region 升降机操作人员

                //apiUrl = "/open/api/reportDeviceStaff";//升降机操作人员
                //var content = HunanTest.GetLifterOperatorContent();
                //#endregion

                //#region 劳务人员数据上传

                //apiUrl = "/open/api/reportStaff";//上传劳务人员信息
                //var content = HunanTest.GetWorkerRegisterContent();

                //#endregion

                //#region 添加项目Id

                ////apiUrl = "/open/api/project/addProjectToken";//添加项目 无效
                ////projectId="5b73796d-7f86-46e3-b743-3d15dfe1ea87",
                ////projectName="测试项目2",

                //#endregion



                #endregion

                #region 重庆

                //host = "http://cqzhgd-api.z023.cn";//"http://cqzhgd-api-test.z023.cn";//1.0 地址
                //port = 80;
                var testHostPrefix = "CS";//"";//测试   正式环境就为空
                host = "http://jsgl.zfcxjw.cq.gov.cn";
                //host = "http://jsgl.zfcxjw.cq.gov.cn:6074/CSIOTWebService/rest/";//测试地址
                //host = "http://jsgl.zfcxjw.cq.gov.cn:6072/IOTWebService/rest/";//正式地址

                port = 6074;//6072;//
                postParameter.Host = host;
                postParameter.Port = port;

                #region 测试人员同步V1.0

                ////var url= "http://cqzhgd-api.z023.cn/api/open/rn/v3/projectWorker";
                //postParameter.Host = "http://cqzhgd-api.z023.cn";
                //postParameter.Port = 80;
                //postParameter.Api = "/api/open/rn/v3/projectWorker";//?serialNo=47dbeba9d8bc50af&pageIndex=0
                //postParameter.Content = ChongqingTest.GetSyncProjectWorkersV1Content("47dbeba9d8bc50af");

                //await ClientGet(postParameter);
                #endregion

                #region 测试获取token
                //////测试： CSIOTWebService/rest/oauth2/token     正式： IOTWebService/rest/oauth2/token      
                ////apiUrl = $"/{testHostPrefix}IOTWebService/rest/oauth2/token";
                ////var tokenResultDto = await ChongqingTest.GetAccessToken($"{postParameter.Host}:{postParameter.Port}",apiUrl);
                ////Console.WriteLine($"{TextJsonConvert.DeserializeObject<object>(TextJsonConvert.SerializeObject(tokenResultDto))}");
                //////Log.Logger.Debug($"{TextJsonConvert.DeserializeObject<object>(TextJsonConvert.SerializeObject(tokenResultDto))}");
                //postParameter.AccessToken = (await ChongqingTest.GetAccessToken($"{postParameter.Host}:{postParameter.Port}", $"/{testHostPrefix}IOTWebService/rest/oauth2/token")).Custom.Access_token;
                //Console.ReadKey();

                //Console.WriteLine($"测试设备操作人员身份识别");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/OperatorDataService/PushOperatorIdentityInfo";
                //postParameter.Content = await ChongqingTest.GetOperatorIdentityContentV2();
                ////Console.WriteLine($"{await postParameter.Content.ReadAsStringAsync()}");
                //await ClientPost(postParameter);


                //Console.ReadKey();


                //Console.WriteLine($"测试项目信息接口");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/BasicInfoDataService/GetProjectInfo";
                //postParameter.Content = await ChongqingTest.GetProjectContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                //Console.WriteLine($"测试项目用工信息接口接口");//BasicInfoDataService
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/RealNameDataService/GetProjectWorkerInfo";
                //postParameter.Content = await ChongqingTest.GetProjectWorkerContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                #endregion


                #region 城发注册
                //host = "https://www.tbaos.cn";
                //port = 443;
                //apiUrl = "/cpms/enterprise/userlist";
                //postParameter.Host = host;
                //postParameter.Port = port;
                //postParameter.Api = apiUrl;
                //postParameter.Content = ChongqingTest.GetChengFaRegisterContent();
                //await ClientPost(postParameter);
                #endregion

                #region 扬尘
                ////apiUrl = "/api/open/iot/v3/env/runtime";
                ////var content = ChongqingTest.GetEnvironmentContent();


                //Console.WriteLine($"测试环境实时数据");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/EnvironmentDataService/PushRealTimeInfo";
                //postParameter.Content = await ChongqingTest.GetEnvironmentContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/EnvironmentDataService/PushLocationInfo";
                //postParameter.Content = await ChongqingTest.GetEnvironmentLocationContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                //Console.WriteLine($"测试环境报警");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/EnvironmentDataService/PushWarnInfo";
                //postParameter.Content = await ChongqingTest.GetEnvironmentWarnContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                //Console.WriteLine($"测试环境喷淋");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/EnvironmentDataService/PushSprayOpenInfo";
                //postParameter.Content = await ChongqingTest.GetEnvironmentSprayContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                #endregion

                #region 塔吊

                ////apiUrl = "/api/open/iot/v3/crane/record";
                ////var content = ChongqingTest.GetCraneContent();


                //Console.WriteLine($"测试塔吊监测基础信息");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/TowerCraneDataService/PushBasicInfo";
                //postParameter.Content = await ChongqingTest.GetCraneBasicContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                //Console.WriteLine($"测试塔吊监测实时数据信息");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/TowerCraneDataService/PushRealTimeInfo";
                //postParameter.Content = await ChongqingTest.GetCraneRealTimeContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                //Console.WriteLine($"测试塔吊监测工作循环数据信息");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/TowerCraneDataService/PushWorkCycleInfo";
                //postParameter.Content = await ChongqingTest.GetCraneWorkCycleContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                //Console.WriteLine($"测试塔吊监测定位数据信息");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/TowerCraneDataService/PushLocationInfo";
                //postParameter.Content = await ChongqingTest.GetCraneLocationContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                //Console.WriteLine($"测试塔吊攀爬防护接口");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/ClimbingTowerCraneDataService/PushClimbingTowerCraneInfo";
                //postParameter.Content = await ChongqingTest.GetClimbingTowerCraneContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                #endregion

                #region 塔吊心跳

                //apiUrl = "/api/open/iot/v3/crane/heartbeat";
                //var content = ChongqingTest.GetCraneHeartbeatContent();

                #endregion

                #region 升降机

                ////apiUrl = "/api/open/iot/v3/lifter/record";
                ////var content= ChongqingTest.GetLifterContent();

                ////apiUrl = "/api/open/iot/v3/lifter/heartbeat";
                ////var content= ChongqingTest.GetLifterHeartbeatContent();



                //Console.WriteLine($"测试附着式升降脚手架基础信息接口");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/SmartClimbDataService/PushSmartClimbInfo";
                //postParameter.Content = await ChongqingTest.GetLifterSmartClimbContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                //Console.WriteLine($"测试升降机监测基础信息");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/ElevatorDataService/PushBasicInfo";
                //postParameter.Content = await ChongqingTest.GetLifterBasicContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();


                //Console.WriteLine($"测试升降机监测实时数据接口规范");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/ElevatorDataService/PushRealTimeInfo";
                //postParameter.Content = await ChongqingTest.GetLifterRealTimeContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                //Console.WriteLine($"测试升降机监测工作循环数据");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/ElevatorDataService/PushWorkCycleInfo";
                //postParameter.Content = await ChongqingTest.GetLifterWorkCycleContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                //Console.WriteLine($"测试升降机监测定位数据");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/ElevatorDataService/PushLocationInfo";
                //postParameter.Content = await ChongqingTest.GetLifterLocationContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                #endregion

                #region 视频

                ////apiUrl = "/api/open/iot/v3/video/info";
                ////var content = ChongqingTest.GetVideoContent();
                //postParameter.Api = "/api/open/iot/v3/video/info";
                //postParameter.Content = ChongqingTest.GetVideoContent();
                //await ClientPost(postParameter);

                //Console.WriteLine($"测试视频监控接口");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/VideoSurveillanceService/PushVideoSurveillanceInfo";
                //postParameter.Content = await ChongqingTest.GetVideoSurveillanceContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();
                #endregion

                #region 考勤

                ////apiUrl = "/api/open/rn/v3/attendance";
                ////var content = ChongqingTest.GetGateContent();

                //Console.WriteLine($"测试智慧工地实名制接口");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/RealNameDataService/UploadAttendanceInfo";
                //postParameter.Content = await ChongqingTest.GetGateContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                #endregion

                #region 安全帽识别

                //postParameter.Host = "http://cqzhgd-api.z023.cn";//正式环境
                //postParameter.Api = "/api/open/iot/v3/helmet/snapshot";
                ////postParameter.Host = "http://dc-api-test.z023.cn";//测试环境
                ////postParameter.Api = "/api/ext/openapi/datacenter/accessHelmetSnapshotData";
                //postParameter.Port = 80;//9000;//测试环境
                //postParameter.Content = ChongqingTest.GetHelmetContent();
                //await ClientPost(postParameter);
                //Console.Read();

                //Console.WriteLine($"测试安全帽预警数据接口");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/SafetyHelmetData/PushSafetyHelmetWarningInfo";
                //postParameter.Content = await ChongqingTest.GetSafetyHelmetWarningContentV2();
                //await ClientPost(postParameter);
                //Console.Read();

                #endregion

                #region 卸料平台

                //Console.WriteLine($"测试卸料平台监测基础信息");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/UnloadingPlatformDataService/PushBasicInfo";
                //postParameter.Content = await ChongqingTest.GetUnloadingPlatformContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                //Console.WriteLine($"测试卸料平台监测实时数据信息");
                //postParameter.Api = $"/{testHostPrefix}IOTWebService/rest/UnloadingPlatformDataService/PushRealTimeInfo";
                //postParameter.Content = await ChongqingTest.GetUnloadingPlatformRealTimeContentV2();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                #endregion

                #region 帝馨车控
                //var loginInput = new
                //{
                //    LoginName = "筑智建科技",//.UrlEncode()//"筑智建科技"
                //    LoginPassword = "123456",
                //    LoginType = "ENTERPRISE",
                //    language = "cn",
                //    ISMD5 = "0",
                //    timeZone = "+08",
                //    apply = "APP",
                //    loginUrl = "http://vipapi.18gps.net/",
                //};
                //postParameter.Host = "http://api.18gps.net";
                //postParameter.Port = 80;
                //postParameter.Api = $"/GetDateServices.asmx/loginSystem?LoginName={loginInput.LoginName}&LoginPassword={loginInput.LoginPassword}&LoginType=ENTERPRISE&language=cn&ISMD5=0&timeZone=+08&apply=APP&loginUrl=http://vipapi.18gps.net/";
                //postParameter.Content = await ChongqingTest.GetVehicleBasicTokenContent();
                //await ClientGet(postParameter);
                //Console.ReadKey();
                //var tResult = await postParameter.Response.Content.ReadAsStringAsync();
                //var json = JsonDocument.Parse(tResult);
                //var mds = json.RootElement.GetProperty("mds").ToString();
                //postParameter.Api = $"/DataService.ashx/GetDate?method=SetGpsUserFullNameAndPlateNumber&macid=868120234141601&fullName=测试车辆1&plateNumber=渝A00000&mds={mds}&LinkName=测试0";
                //Console.WriteLine(postParameter.GetUrl);
                //await ClientGet(postParameter);
                //Console.ReadKey();
                #endregion

                #endregion

                #region 贵阳


                host = "http://221.13.13.133";//"http://222.85.156.48"

                port = 21974;//17000
                postParameter.Host = host;
                postParameter.Port = port;

                #region 分包单位企业信息接口

                //Console.WriteLine($"测试分包单位企业信息接口");
                //postParameter.Api = $"/api/service/subenterprise/saveOrUpdate";
                //postParameter.Content = await GuizhouTest.GetSubEnterpriseContent();
                //await ClientPost(postParameter);
                //Console.Read();

                #endregion

                #region 前端设备心跳/扬尘塔吊施工升降机设备心跳

                //Console.WriteLine($"扬尘塔吊施工升降机设备心跳接口");
                //postParameter.Api = $"/api/service/DeviceHeartbeat/save";//$"/api/service/heartBeat/save";
                //postParameter.Content = await GuizhouTest.GetHeartbeatContent("serialNo");
                //await ClientPost(postParameter);
                //Console.Read();

                #endregion

                #region 扬尘

                //Console.WriteLine($"测试扬尘监测接口");
                //postParameter.Api = $"/api/service/RaiseDustMonitor/saveOrUpdate";
                //postParameter.Content = await GuizhouTest.GetEnvironmentContent();
                //await ClientPost(postParameter);
                //Console.Read();

                #endregion

                #region 塔吊

                //Console.WriteLine($"测试塔吊在线监测接口");
                //postParameter.Api = $"/api/service/TowerCraneMonitor/saveOrUpdate";
                //postParameter.Content = await GuizhouTest.GetCraneRealTimeContent();
                //await ClientPost(postParameter);
                //Console.Read();

                #endregion

                #region 升降机

                //Console.WriteLine($"测试施工升降机智能监测接口");
                //postParameter.Api = $"/api/service/ElevatorMonitor/saveOrUpdate";
                //postParameter.Content = await GuizhouTest.GetLifterRealTimeContent();
                //await ClientPost(postParameter);
                //Console.Read();

                #endregion

                #region 考勤

                //Console.WriteLine($"测试考勤接口");
                //postParameter.Api = $"/api/service/attendance/save";
                //postParameter.Content = await GuizhouTest.GetGateContent();
                //await ClientPost(postParameter);
                //Console.Read();

                #endregion

                #region 人员基本信息接口

                //Console.WriteLine($"测试人员基本信息接口");
                //postParameter.Api = $"/api/service/person/saveOrUpdate";
                //postParameter.Content = await GuizhouTest.GetGateRegisterContent();
                //await ClientPost(postParameter);
                //Console.Read();

                #endregion

                #region 班组信息接口

                //Console.WriteLine($"测试班组信息接口");
                //postParameter.Api = $"/api/service/team/saveOrUpdate";
                //postParameter.Content = await GuizhouTest.GetTeamContent();
                //await ClientPost(postParameter);
                //Console.Read();

                #endregion

                #region 从业人员入职信息接口

                //Console.WriteLine($"测试从业人员入职信息接口");
                //postParameter.Api = $"/api/service/person/postSaveOrUpdate";
                //postParameter.Content = await GuizhouTest.GetPersonPostContent();
                //await ClientPost(postParameter);
                //Console.Read();

                #endregion

                #region 从业人员离职信息接口

                //Console.WriteLine($"测试从业人员离职信息接口");
                //postParameter.Api = $"/api/service/person/quitSaveOrUpdate";
                //postParameter.Content = await GuizhouTest.GetPersonQuitContent();
                //await ClientPost(postParameter);
                //Console.Read();

                #endregion


                #region 从业人员资质信息接口

                //Console.WriteLine($"测试从业人员资质信息接口");
                //postParameter.Api = $"/api/service/person/qualicationSaveOrUpdate";
                //postParameter.Content = await GuizhouTest.GetQualicationContent();
                //await ClientPost(postParameter);
                //Console.Read();

                #endregion

                #region 劳务人员工资接口

                //Console.WriteLine($"测试劳务人员工资接口");
                //postParameter.Api = $"/api/service/salary/saveOrUpdate";
                //postParameter.Content = await GuizhouTest.GetSalaryContent();
                //await ClientPost(postParameter);
                //Console.Read();

                #endregion

                #region 项目工资接口

                //Console.WriteLine($"测试项目工资接口");
                //postParameter.Api = $"/api/service/projectSalary/saveOrUpdate";
                //postParameter.Content = await GuizhouTest.GetProjectSalaryContent();
                //await ClientPost(postParameter);
                //Console.Read();

                #endregion

                #region 劳务合同接口

                //Console.WriteLine($"测试劳务合同接口");
                //postParameter.Api = $"/api/service/contract/saveOrUpdate";
                //postParameter.Content = await GuizhouTest.GetContractContent();
                //await ClientPost(postParameter);
                //Console.Read();

                #endregion


                #endregion

                #region 中冶赛迪
                //host = "http://api-device.qingzhuyun.com";//正式环境
                //port = 80;
                //postParameter.Host = host;
                //postParameter.Port = port;
                #region 添加组织机构和班组
                //apiUrl = "/device/laborInfo/insertOrg";
                //var content = CisdiTest.AddOrganization();
                //var content = CisdiTest.AddClass();
                //postParameter.Api = "/device/laborInfo/insertOrg";
                //postParameter.Content = CisdiTest.AddOrganization();//CisdiTest.AddClass()
                //await ClientPost(postParameter);
                #endregion

                #region 现场人员新增 (实名制采集)门禁

                //apiUrl = "/device/laborInfo/laborSave";
                //var content = CisdiTest.GetWorkerRegisterContent();
                //postParameter.Api = "/device/laborInfo/laborSave";
                //postParameter.Content = CisdiTest.GetWorkerRegisterContent();
                //await ClientPost(postParameter);
                #endregion

                #region 现场人员删除 

                //apiUrl = "/device/laborInfo/laborDelete";
                //var content = CisdiTest.GetWorkerDeleteContent();

                #endregion

                #region 考勤(门禁)
                //apiUrl = "/device/guard/recordListNew";
                //var content = CisdiTest.GetGateContent();

                #endregion

                #region 环境监测

                //apiUrl = "/device/environment/pubin";
                //var content = CisdiTest.GetEnvironmentContent();
                #endregion

                #region 塔机运行数据

                //apiUrl = "/device/crane/pubin";
                //var content = CisdiTest.GetCraneContent();
                #endregion

                #region 塔机吊装数据 (攀爬)

                //apiUrl = "/device/climbBelt/pubin";
                //var content = CisdiTest.GetCraneWorkCycleContent();
                #endregion

                #region 升降机

                //apiUrl = "/device/elevator/pubin";
                //var content = CisdiTest.GetLifterContent();
                #endregion

                #endregion

                #region 成都环境建设信息平台

                //host = "http://vh9gb74u.xiaomy.net";//正式：http://120.24.214.246:8888
                //port = 80;//8888

                #region 考勤

                //apiUrl = "/ktfw-boot/ktapi/attendance/log";

                //var content = OtherTest.GetGateContent();

                #endregion

                #region 环境

                //apiUrl = "/ktfw-boot/ktapi/envSample/data";

                //var content = OtherTest.GetEnvironmentContent();

                #endregion

                #endregion

                #region 云筑智联

                host = "https://ibuildapi.yzw.cn/open.api";
                port = 443;
                postParameter.Host = host;
                postParameter.Port = port;
                postParameter.Api = "";
                ////postParameter.Api = "/upload.envMonitorLiveData"; 
                ////string url = postParameter.GetUrl;
                ////var array = url.Split("/upload");
                ////Console.WriteLine($"{url}");
                ////Console.WriteLine($"{array?.First()}");
                ////Console.WriteLine($"{array?.Last()}");
                ////Console.WriteLine($"{url?.Substring(0, url?.LastIndexOf("/")??0)}");

                #region 环境

                //postParameter.Content = await YunzhuTest.GetEnvironmentContent();
                //await ClientPost(postParameter);

                #endregion

                #region 自动降尘喷淋心跳 官网未提供此接口调用

                //postParameter.Content = await YunzhuTest.GetHeartbeatContent();
                //await ClientPost(postParameter);

                #endregion

                #region 塔吊

                //postParameter.Content = await YunzhuTest.GetCraneContent();
                //await ClientPost(postParameter);
                //postParameter.Content = await YunzhuTest.GetCraneLiveContent(); 
                //await ClientPost(postParameter);

                #endregion

                #region 升降机

                //postParameter.Content = await YunzhuTest.GetLifterContent();
                //await ClientPost(postParameter);
                //postParameter.Content = await YunzhuTest.GetLifterLiveContent(); 
                //await ClientPost(postParameter);

                #endregion

                #region 卸料平台

                //postParameter.Content = await YunzhuTest.GetUnloadingContent();
                //await ClientPost(postParameter);

                //postParameter.Content = await YunzhuTest.GetUnloadingLiveContent();
                //await ClientPost(postParameter);

                #endregion

                #region 上传进度

                //postParameter.Content = await YunzhuTest.GetProcessTaskDataContent();
                //await ClientPost(postParameter);

                #endregion

                #region 上传工程量汇总 官网未提供此接口调用

                //postParameter.Content = await YunzhuTest.GetPartProgressSummaryDataContent();
                //await ClientPost(postParameter);

                #endregion

                #region 上传工程量和产值完成情况（按月） 官网未提供此接口调用

                //postParameter.Content = await YunzhuTest.GetPartProgressMonthDataContent();
                //await ClientPost(postParameter);

                #endregion

                #region 上传工程量和产值完成情况（按天）官网未提供此接口调用

                //postParameter.Content = await YunzhuTest.GetPartProgressDayDataContent();
                //await ClientPost(postParameter);

                #endregion

                #region 上传质量 {"data":null,"code":1,"message":"上传数据不能为空"}

                //postParameter.Content = await YunzhuTest.GetQualityCheckDataContent();
                //await ClientPost(postParameter);

                #endregion

                #region 上传安全数据  {"data":null,"code":1,"message":"上传数据不能为空"}

                //postParameter.Content = await YunzhuTest.GetSecurityCheckDataContent();
                //await ClientPost(postParameter);

                #endregion

                #region 吊篮

                //postParameter.Content = await YunzhuTest.GetHangingBasketContent();
                //await ClientPost(postParameter);

                //postParameter.Content = await YunzhuTest.GetHangingBasketLiveDataContent();
                //await ClientPost(postParameter);

                #endregion

                #region 电表

                //postParameter.Content = await YunzhuTest.GetElectricityMeterContent();
                //await ClientPost(postParameter);

                #endregion

                #region 水表

                //postParameter.Content = await YunzhuTest.GetWaterMeterContent();
                //await ClientPost(postParameter);

                #endregion

                #region 养护室监测

                //postParameter.Content = await YunzhuTest.GetCuringRoomLiveDataContent();
                //await ClientPost(postParameter);

                #endregion

                #region 烟感监测

                //postParameter.Content = await YunzhuTest.GetSmokeDetectorContent();
                //await ClientPost(postParameter);

                #endregion

                #region 越界监测

                //postParameter.Content = await YunzhuTest.GetIntrusionDetectorContent();
                //await ClientPost(postParameter);

                #endregion

                #region 混凝土测温数据

                //postParameter.Content = await YunzhuTest.GetConcreteContent();
                //await ClientPost(postParameter);

                #endregion

                #region 排污监测

                //postParameter.Content = await YunzhuTest.GetSewageOutfallContent();
                //await ClientPost(postParameter);

                #endregion

                #region 雨水回收

                //postParameter.Content = await YunzhuTest.GetRainRecoveryContent();
                //await ClientPost(postParameter);

                #endregion

                #region 智慧停车

                //postParameter.Content = await YunzhuTest.GetVehicleManagementContent();
                //await ClientPost(postParameter);

                #endregion


                #endregion

                #region 厦门会展一标段

                host = "http://123.56.74.49";
                port = 6000;
                //var sectionOneDeviceId = "870699588397301760";
                postParameter.Host = host;
                postParameter.Port = port;
                //////环境实时数据接口
                //postParameter.Api = $"/devices/{sectionOneDeviceId}/data/latest";//870699588397301760
                ////postParameter.Content = OtherTest.GetXiamenSectionOneEnvironmentRealtimeContent();
                //await ClientGet(postParameter);
                //Console.ReadKey();

                ////环境数据列表接口 每次请求100条，pageIndex递增，直到请求最后一页pageSize<100完毕
                //postParameter.Api = $"/devices/{sectionOneDeviceId}/data/page?pageSize=100&pageIndex=0";//870699588397301760

                ////await ClientGet(postParameter);
                ////Console.ReadKey();


                //考勤记录数据列表接口 每次请求100条，pageIndex递增，直到请求最后一页pageSize<100完毕
                //sectionOneDeviceId = "887344297819504640";
                //postParameter.Api = $"/services/execute/GetProjectAttendanceListNew?deviceId={sectionOneDeviceId}";//887344297819504640
                //postParameter.Content = OtherTest.GetXiamenSectionOneGateHistoryContent();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                #endregion

                #region 厦门会展二标段

                #region 奥斯恩(扬尘)设备数据接口测试

                host = "http://47.107.103.35";
                port = 8010;

                postParameter.Host = host;
                postParameter.Port = port;

                //postParameter.Api = "/openApi/data/realtime";// |realtime|minute|hour|day
                ////2021060203100003对应的sn是 ：MjAyMTA2MDIwMzEwMDAwMw==
                //postParameter.Content = OtherTest.GetAoSienEnvironmentRealtimeContent();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                ////历史数据 查询时间不能超过7天，避免影响服务器运行效率
                //postParameter.Api = "/openApi/his/data/realtime";// |realtime|minute|hour|day
                //postParameter.Content = OtherTest.GetAoSienEnvironmentHistoryContent();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                #endregion

                #region 工人打卡记录

                {

                    //host = "https://glm.glodon.com/api/open";//SERVICE-ADDRESS   服务域名？glm.glodon.com/api/open
                    //port = 443;
                    //postParameter.Host = host;
                    //postParameter.Port = port;
                    ////这里根据appid、secret计算出签名sign 放在url里面或者post参数里面请求接口
                    //var secret = "bd1b6322217dfbeb96de3696d1a387fa";
                    //var gateInput = new
                    //{
                    //    appid = "0a9416fd6baa4595a1268ad1f52034b0",
                    //    //secret = "bd1b6322217dfbeb96de3696d1a387fa",
                    //    //sign = "",//调用一个方法计算sign
                    //    beginDate = $"{DateTime.Now.AddDays(-1):yyyy-MM-dd}",
                    //    endDate = $"{DateTime.Now:yyyy-MM-dd}",
                    //    inOutType = "IN",//IN进，OUT出
                    //    projectId = "490952877121536",//租户id：1438415,
                    //    startId = 0,//3397840564,//
                    //    pageSize = 10
                    //};
                    ////调用一个方法计算sign
                    ////var sign = $"{gateInput.secret} appid{appid} beginDate{beginDate} {gateInput.secret}";
                    ////var sign = MD5Encrypt.Encrypt($"{json}");
                    //var sign = Utilities.GetXiamenSectionTwoSign(secret, gateInput);


                    ////考勤记录数据列表接口 每次请求100条，pageIndex递增，直到请求最后一页pageSize<100完毕
                    //postParameter.Api = $"/attendance/cardV2?appid={gateInput.appid}&sign={sign}&beginDate={gateInput.beginDate}&endDate={gateInput.endDate}&inOutType={gateInput.inOutType}&projectId={gateInput.projectId}&startId={gateInput.startId}&pageSize={gateInput.pageSize}";
                    //await ClientGet(postParameter);

                    //Console.ReadKey();
                }
                #endregion

                #endregion

                #region 沙区管网

                //host = "http://ip";
                //port = 6000;
                //postParameter.Host = host;
                //postParameter.Port = port;
                ////info
                //postParameter.Api = "/dxbsp/findByMap";
                //postParameter.Content = OtherTest.GetShaquGuanwangDeviceInfosContent();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                ////history
                //postParameter.Api = $"/history/findByMap?startTime={DateTime.Now.AddDays(-1).ToLongTimeString()}&endTime={DateTime.Now.ToLongTimeString()}";//ToUnixTimeTicks(13)
                //postParameter.Content = OtherTest.GetShaquGuanwangDeviceInfosContent();
                //await ClientPost(postParameter);
                //Console.ReadKey();

                ////location
                //postParameter.Api = $"/history/findByMap";
                //postParameter.Content = OtherTest.GetShaquGuanwangDeviceInfosContent();
                //await ClientPost(postParameter);
                //Console.ReadKey();
                #endregion

                #region client

                //await ClientPost(postParameter);

                #endregion

                #region 人脸识别设备配置

                host = "http://172.16.4.199";
                port = 8090;

                postParameter.Host = host;
                postParameter.Port = port;

                #region 获取设备序列号

                //postParameter.Api = "/getDeviceKey";
                //postParameter.Content = OtherTest.GetGateSerialNoContent();
                //await ClientPost(postParameter);
                #endregion

                #region 设置设备密码

                //postParameter.Api = "/setPassWord";
                //postParameter.Content = OtherTest.SetGatePasswordContent();
                //await ClientPost(postParameter);
                //#endregion

                #region 获取设备人员列表api

                ////postParameter.Api = "/person/findByPage?pass=12345678&personId=-1&length=1000&index=0";
                ////postParameter.Content = OtherTest.GetPersonByPageContent();
                ////await ClientGet(postParameter);
                #endregion

                //#region 删除所有人员api

                ////postParameter.Api = "/person/delete";
                ////postParameter.Content = OtherTest.PersonDeleteAll();
                ////await ClientPost(postParameter);
                #endregion

                #region 设置设备心跳回调api

                //postParameter.Api = "/setDeviceHeartBeat";
                //postParameter.Content = OtherTest.SetGateHeartBeatContent();
                //await ClientPost(postParameter);
                #endregion

                #region 设置设备获取任务api

                //postParameter.Api = "/setTaskInterfaceAddress";
                //postParameter.Content = OtherTest.SetGateGetTaskContent();
                //await ClientPost(postParameter);
                #endregion

                #region 设置设备任务处理结果回调api

                //postParameter.Api = "/setTaskProcessingResultsAddress";
                //postParameter.Content = OtherTest.SetGateHandleTaskResultContent();
                //await ClientPost(postParameter);
                #endregion

                #region 设置设备识别回调api

                //postParameter.Api = "/setIdentifyCallBack";
                //postParameter.Content = OtherTest.SetGateIdentifyCallBackContent();
                //await ClientPost(postParameter);
                #endregion


                #endregion

                #region 直接调用Grpc

                //await TestHello();
                //await TestMath();

                //var now = DateTime.Now;
                //Console.WriteLine($"{now.ToUnixTimeTicks(13)- 1599000000000}");
                //Console.WriteLine($"{1599000000.ToLocalDateTime()}");
                //var time = DateTime.Now;
                //var startTime = TimeZoneInfo.ConvertTime(new DateTime(time.Year, 1, 1), TimeZoneInfo.Local);
                //var period = time - startTime;
                //Console.WriteLine($"{period.TotalMilliseconds}");

                //var items = new List<int> { 1, 2, 3 };
                //foreach (var item in items)
                //{
                //    Console.WriteLine($"等待前：{DateTime.Now.Second}");
                //    await Task.Delay(3000);
                //    Console.WriteLine($"等待后：{DateTime.Now.Second}");
                //}

                //for (int i = 0; i < 3; i++)
                //{
                //    await Task.Delay(new Random().Next(1000, 5000));//在这里等待1-3s即可生成不同的ticks
                //    var now = DateTime.Now;
                //    Console.WriteLine($"10位数时间戳第{i}次：{now.ToUnixTimeTicks()}");
                //    Console.WriteLine($"int{i}：{(int)now.ToUnixTimeTicks()}");
                //    Console.WriteLine($"now：{now}");
                //    Console.WriteLine($"now.FromUnix：{DateTimeExtensions.FromUnix(now.ToUnixTimeTicks())}");
                //}

                //var personnelId = "d9ce368d48e74b37a7b47d473c1af9aa";//"a16416d6bda442d186ee44b8d50b0b9e";// Guid.NewGuid().ToString("N");//
                //var idCard = "500223199005231751";//"500108199003075012";//"500223199705231751";//
                //var fakeNo = "51010710050002";
                //var client = new ReverseControlClient();
                //GrpcOption.Instance.Initialize("archgl", "device", "iot_appsecret", "https://reverse.spdyun.cn");// "https://localhost:5003";//开发环境

                //Console.WriteLine($"请求{ReverseControlCommandClient.ServerUrl},{ReverseControlCommandClient.AppId},{ReverseControlCommandClient.AppKey},{ReverseControlCommandClient.AppId}");

                #region 设备实名制注册+添加人员与添加照片+删除人员

                //var model = new Common.ROClient.Models.PersonRegisterModel
                //{
                //    Address = "重庆沙坪坝",
                //    Birthday = "1997-05-23",
                //    Gender = GenderType.Male,
                //    //PersonnelType = 1,
                //    //RegisterType = 3,
                //    IdCard = idCard,
                //    IcCard = "123456789",
                //    //IdPhoto = Utilities.Base64Phto2.RemoveBase64ImagePrefix(), //"123",//
                //    Photo = Utilities.Base64Phto.RemoveBase64ImagePrefix(),
                //    //InfraredPhoto = Utilities.Base64Phto3.RemoveBase64ImagePrefix(),
                //    IssuedBy = "筑智建研发部",
                //    Name = "测试1",
                //    Nation = "汉",
                //    PersonnelId = personnelId,
                //    PhoneNumber = "17772433921",
                //    TermValidityStart = "19970523",
                //    TermValidityEnd = "21990719",
                //};
                //Console.WriteLine($"Json序列化input:");
                //Console.WriteLine($"{Newtonsoft.Json.JsonConvert.DeserializeObject(TextJsonConvert.SerializeObject(model))}");
                //var replay = await client.ExecutePersonRegisterCommandAsync(fakeNo, model);////"35021210050001"

                //Console.WriteLine($"人员实名制注册{personnelId}:{replay.Message}");
                //Console.ReadKey();

                //var replay = await client.ExecutePersonAddCommandAsync(fakeNo, new PersonAddedModel
                //{
                //    Name = "测试0",
                //    IdCard = idCard,
                //    IcCard = "123456789",
                //    PersonnelId = personnelId,
                //    Faces = new List<FaceModel>
                //    {
                //        new FaceModel
                //        {
                //            Image = Utilities.Base64Phto3.RemoveBase64ImagePrefix(),
                //            FaceId = personnelId//Guid.NewGuid().ToString("N")
                //        }
                //    }
                //});

                //Console.WriteLine($"向设备{fakeNo}下发人员与照片:{personnelId}:{replay.Message}");
                //Console.ReadLine();

                //replay = await client.ExecutePersonDeleteCommandAsync(fakeNo, new PersonDeletedModel
                //{
                //    PersonnelIds = new[] { personnelId }
                //});
                //Console.WriteLine($"从设备{fakeNo}删除人员{personnelId}:{replay.Message}");
                //Console.ReadLine();
                #endregion


                #endregion

                #region 测试IOT接口 Standard


                host = "http://api.iot.sunbl.com";//"http://iot.spddemo.com";//"http://localhost";//"http://6.6.6.152";//网关"https://api.iot.test.spdio.com"; //"https://api.spdyun.cn";//
                port = 80;//8008;//443;//

                postParameter.Host = host;
                postParameter.Port = port;

                #region 计算token

                //var timeStamp1 = DateTime.Now.ToUnixTimeTicks();
                //var timeStamp2 = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                //var timeStamp3 = (DateTime.Now.Ticks - 621355968000000000) / 10000000;
                //var timeStamp = timeStamp1;
                //Console.WriteLine($"timeStamp1={timeStamp1},timeStamp2={timeStamp2},timeStamp3={timeStamp3}");
                //var _tokenDto = new AppTokenInputDto
                //{
                //    AppId = "kczx",//"archglId",
                //    AppKey = "device",//"archglKey",
                //    AppSecret = "iot_jA9HFJQz",//"archglSecret",
                //    TimeStamp = timeStamp.ToString()
                //};
                //await Utilities.GetIotToken(_tokenDto);

                //Console.WriteLine($"AppId:{_tokenDto.AppId}");
                //Console.WriteLine($"AppKey:{_tokenDto.AppKey}");
                //Console.WriteLine($"AppToken:{_tokenDto.Token}");
                //Console.WriteLine($"TimeStamp:{_tokenDto.TimeStamp}");
                //Console.ReadKey();


                #endregion

                #region 测试IOT基坑全站仪接口

                //postParameter.Api = "/device/v2/Foundationpit/TotalstationRecords";
                ////postParameter.ReturnVoid = true;
                //postParameter.Content = await IOTTest.GetFoundationpitContent();
                //await ClientPost(postParameter);

                #endregion

                #region 测试IOT环境接口

                //postParameter.Api = "/device/v2/Environment/Record";//网关device/*-->api/*
                ////postParameter.ReturnVoid = true;
                //postParameter.Content = await IOTTest.GetEnvironmentContent();
                //await ClientPost(postParameter);

                #endregion

                #region 测试IOT闸机接口

                //postParameter.Api = "/device/v2/gate/Record";
                ////postParameter.ReturnVoid = true;
                //postParameter.Content = await IOTTest.GetGateContent();
                //await ClientPost(postParameter);

                #endregion

                #region 测试IOT心跳接口

                //postParameter.Api = "/device/v2/Environment/heartbeat";
                //postParameter.ReturnVoid = true;
                //postParameter.Content = await IOTTest.GetHeartbeatContent("1234569999");
                //await ClientPost(postParameter);

                //postParameter.Api = "/device/v2/gate/heartbeat"; 
                //postParameter.ReturnVoid = true;
                //postParameter.Content = await IOTTest.GetHeartbeatContent("99999910050001");
                //await ClientPost(postParameter);

                //postParameter.Api = "/device/v2/gate/heartbeat";
                //postParameter.ReturnVoid = true;
                //postParameter.Content = await IOTTest.GetHeartbeatContent("99999910050002");
                //await ClientPost(postParameter);

                //postParameter.Api = "/device/v2/crane/heartbeat";
                //postParameter.ReturnVoid = true;
                //postParameter.Content = await IOTTest.GetHeartbeatContent("99999910030001");
                //await ClientPost(postParameter);

                #endregion

                #region 测试IOT塔吊接口

                //postParameter.Api = "/device/v2/crane/basic";
                ////postParameter.ReturnVoid = true;
                //postParameter.Content = await IOTTest.GetCraneBasicContent();
                //await ClientPost(postParameter);

                //await Task.Delay(1000);

                //postParameter.Api = "/device/v2/crane/driver";
                ////postParameter.ReturnVoid = true;
                //postParameter.Content = await IOTTest.GetCraneDriverContent();
                //await ClientPost(postParameter);

                //await Task.Delay(1000);
                //postParameter.Api = "/device/v2/crane/record";
                ////postParameter.ReturnVoid = true;
                //postParameter.Content = await IOTTest.GetCraneRecordContent();
                //await ClientPost(postParameter);

                //await Task.Delay(1000);
                //postParameter.Api = "/device/v2/crane/cycle";
                ////postParameter.ReturnVoid = true;
                //postParameter.Content = await IOTTest.GetCraneCycleContent();
                //await ClientPost(postParameter);

                #endregion

                #region 测试IOT人员注册

                //postParameter.Api = "/device/v2/worker/register";
                ////postParameter.ReturnVoid = true;
                //postParameter.Content = await IOTTest.GetWorkerRegisterContent();
                //await ClientPost(postParameter);

                //postParameter.Api = "/device/v2/gate/heartbeat";
                //postParameter.Content = await IOTTest.GetHeartbeatContent("35021210050001");
                //await ClientPost(postParameter);
                #endregion

                #region 测试IOT 管网设备接口

                //postParameter.Api = "/device/v2/water/qualityRecord";
                ////postParameter.ReturnVoid = true;
                //IOTTest.UpdateTokenDto("pipenet", "iot_pipenet", "iot_71FAD6A46");
                //postParameter.Content = await IOTTest.GetWaterQualityContent();
                //await ClientPost(postParameter);

                #endregion

                #region 调用iot_ops 接口 判断

                ////1、获取token
                //var tokenString = await IOTTest.GetIotOpsToken();
                //var token = TextJsonConvert.DeserializeObject<IotOpsResultDto>(tokenString);

                //postParameter.Host = "https://localhost";//"https://ops.spdyun.cn";
                //postParameter.Port = 5001;
                //postParameter.Api = "/device/checkFeedBackAndReSendWorkerDevice";
                //postParameter.AccessToken = token.Access_token;//header里面要写token信息
                //postParameter.Content = await IOTTest.GetRequestWithTokenContent(token.Access_token);
                //await ClientPost(postParameter);
                //Console.ReadKey();

                #endregion

                #endregion

            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
            //Console.WriteLine($"requrest result:{response != null && response.StatusCode is HttpStatusCode.OK}");

        }



        #region 自定义Configuration 热更新
        {
            ////自定义Configuration 每间隔一定时间重新加载 热更新
            //var builder = new ConfigurationBuilder();
            //builder.AddMyConfiguration();//builder.Add(new MyConfigurationSource());
            //var configRoot = builder.Build();
            //ChangeToken.OnChange(() => configRoot.GetReloadToken(), () =>
            //{
            //    Console.WriteLine($"lastTime：{configRoot["lastTime"] ?? "不存在lastTime"}");
            //});
            //Console.WriteLine("开始了");
            //Console.ReadKey();
        }
        #endregion

        #region 数据结构与算法
        {
            #region 数组Array 连续，节约空间，查找快，增删慢 定长多维数组 矩阵数组 锯齿数组
            //动态数组ArrayList 变长   List就是ArrayList 加了泛型 
            var arrayList = new ArrayList();//容量参数Capacity 不传默认会有4的长度
            arrayList.Add("sdfsdf1");
            arrayList.Add("sdfsdf2");
            arrayList.RemoveAt(0);//开辟空间--copy后面的元素到一个新的数组
                                  //stack 内部都是数组
            Stack<string> stack = new Stack<string>();
            stack.Push("1");
            stack.Peek();
            stack.Pop();
            //queue 内部都是数组
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(1);
            queue.Peek();
            queue.Dequeue();

            #endregion
            //链表 就是一个类，属性指向其它实例，然后串起来 用得很少
            //var linkedList = new LinkedList<string>();
            //hash  key-value 线程安全--object装箱拆箱 增删改查都很快(相对数组和连接)



        }
        #endregion

        #endregion

        #region abp 调用测试

        {
            //using var application = AbpApplicationFactory.Create<AppModule>(options =>
            //{
            //    options.UseAutofac(); //Autofac integration
            //});
            //application.Initialize();
            //// 解析服务并使用它
            //var helloWorldService =
            //    application.ServiceProvider.GetService<HelloWorldService>();

            //helloWorldService.SayHello();
            //Console.WriteLine("Press ENTER to stop application...");
            //Console.ReadLine();
        }
        #endregion

        #region 动态代理（AOP拦截器）

        {
            //逻辑还原：
            //1、有一个基础的接口定义规范
            //2、想办法让用户传一个啥进来，然后，不改变原来函数的动作的情况下，我给这个函数增加一个啥功能
            //3、可能得有注册，因为我也不知道代理啥东西
            var aopTest = new AopTest();

            var res = aopTest.Register(typeof(IProxyAopTest));

            var res1 = (IProxyAopTest)res;
            res1.Do();
        }
        #endregion

        #region 拆解含实体类型字段的表达式目录是结构：Expression<Func<LocationArea, bool>> exp = (m) => m.Id.ToString().Equals("5");

        {

            ////Expression<Func<LocationArea, bool>> exp = (m) => m.Id.ToString().Equals("5");
            ////拆解步骤：m m.Id  m.Id.ToString()   m.Id.ToString().Equals("5");
            //var m = Expression.Parameter(typeof(LocationArea), "m");//m参数为LocationArea类型
            //var strExp = Expression.Constant("5");//"5" 的表达式
            //var field = typeof(LocationArea).GetProperty("Id");//typeof(LocationArea).GetField("Id");
            //var fieldExp = Expression.Property(m, field);//m.Id 的表达式 Expression.Field(m, field)
            //var toString = typeof(int).GetMethod("ToString", new Type[] { });//对象的无参数的ToString()方法
            //var toStringExp = Expression.Call(fieldExp, toString, new Expression[0]);//m.Id.ToString() 无参数的表达式目录树表示
            //var equals = typeof(string).GetMethod("Equals", new Type[] { typeof(string) });//sting类型的Equals("参数")参数为string类型
            //var equalsExp = Expression.Call(toStringExp, equals, new Expression[] { strExp });//m.Id.ToString().Equals("5") 的表达式目录树
            //var exp = Expression.Lambda<Func<LocationArea, bool>>(equalsExp, new ParameterExpression[] { m });
            //var result = exp.Compile().Invoke(new LocationArea
            //{
            //    //Id = 5,
            //    Name = "测试"
            //});
            ////exp.Compile()(new LocationArea{Id = 5,Name = "测试"});//只能执行表示Lambda表达式的表达式目录树，即LambdaExpression或者Expression<TDelegate>类型。如果表达式目录树不是表示Lambda表达式，需要调用Lambda方法创建一个新的表达式

            //Console.WriteLine($"m.Id==5表达式目录树执行返回结果:{result}");

        }
        //{
        //    //拼装表达式目录树，交给下端用
        //    var parameterExp = Expression.Parameter(typeof(LocationArea), "m");
        //    var propertyExp = Expression.Property(parameterExp, typeof(LocationArea).GetProperty("Level"));
        //    var constantExp = Expression.Constant(18, typeof(int));
        //    var binary = Expression.GreaterThan(propertyExp, constantExp);//Level>5
        //    var lambda = Expression.Lambda<Func<LocationArea, bool>>(binary, new ParameterExpression[] { parameterExp });//m.Level>5
        //    var boolResult = lambda.Compile()(new LocationArea { Level = TreeNodeLevel.五级 });
        //    Console.WriteLine($"m.Level>5表达式目录树执行返回结果:{boolResult}");
        //}


        #endregion

        #region 利用拆解表达式目录树编写Mapper 性能测试 

        {

            People people = new People()
            {
                Id = 11,
                Name = "Eleven",
                Age = 31
            };
            PeopleCopy peopleCopy = new PeopleCopy()
            {
                Id = people.Id,
                Name = people.Name,
                Age = people.Age
            };

            var pepCopy = ExpMapper<People, PeopleCopy>.Trans(people);

            LocationArea LocationArea = new LocationArea
            {
                //Id = Guid.NewGuid(),
                Name = "Chensq",
            };


            //var result = ExpMapper<LocationArea, LocationAreaDto>.Trans(LocationArea);
            //Console.WriteLine($"泛型表达式目录树映射返回结果:{result.Name}");
            //result = ExpMapper<LocationArea, LocationAreaDto>.Trans(new LocationArea { Name = "Test2" });
            //Console.WriteLine($"泛型表达式目录树映射返回结果:{result.Name}");

            //long common = 0;
            //long generic = 0;
            //long cache = 0;
            //long reflection = 0;
            //long serialize = 0;
            //var dtoList = new List<LocationAreaDto>();
            //{
            //    Stopwatch watch = new Stopwatch();
            //    watch.Start();
            //    for (int i = 0; i < 100000; i++)
            //    {
            //        var copy = new LocationAreaDto()
            //        {
            //            Id = LocationArea.Id,
            //            Name = LocationArea.Name,
            //            Level = LocationArea.Level
            //        };
            //    }
            //    watch.Stop();
            //    common = watch.ElapsedMilliseconds;
            //}
            //{
            //    Stopwatch watch = new Stopwatch();
            //    watch.Start();
            //    for (int i = 0; i < 100000; i++)
            //    {
            //        var copy = ExpMapper<LocationArea, LocationAreaDto>.Trans(LocationArea);
            //    }
            //    watch.Stop();
            //    generic = watch.ElapsedMilliseconds;
            //}
            ////{
            ////    Stopwatch watch = new Stopwatch();
            ////    watch.Start();
            ////    for (int i = 0; i < 100000; i++)
            ////    {
            ////        var copy = ReflectionMapper.Trans<LocationArea, LocationAreaDto>(LocationArea);
            ////    }
            ////    watch.Stop();
            ////    reflection = watch.ElapsedMilliseconds;
            ////}
            //{
            //    Stopwatch watch = new Stopwatch();
            //    watch.Start();
            //    for (int i = 0; i < 100000; i++)
            //    {
            //        dtoList.Add(SerializeMapper.Trans<LocationArea, LocationAreaDto>(LocationArea));
            //    }
            //    watch.Stop();
            //    serialize = watch.ElapsedMilliseconds;
            //}
            //{
            //    Stopwatch watch = new Stopwatch();
            //    watch.Start();
            //    for (int i = 0; i < 100000; i++)
            //    {
            //        var copy = ExpressionMapper.Trans<LocationArea, LocationAreaDto>(LocationArea);
            //    }
            //    watch.Stop();
            //    cache = watch.ElapsedMilliseconds;
            //}

            //Console.WriteLine($"common = { common} ms");
            //Console.WriteLine($"generic = { generic} ms");
            ////Console.WriteLine($"reflection = { reflection} ms");
            //Console.WriteLine($"serialize = { serialize} ms");
            //Console.WriteLine($"cache = { cache} ms");
        }

        #endregion

        #region 拼装表达式目录树 数据结构

        {
            //sql查询的时候，各种查询条件
            //属性可能Id，Name...
            //Expression<Func<LocationArea,bool>> lamda = x => x.Level > 5;


        }

        #endregion

        #region 拼装表达式目录树 应用 表达式链接

        {
            //Expression<Func<People, bool>> lambda1 = x => x.Age > 5;
            //Expression<Func<People, bool>> lambda2 = x => x.Id > 5;
            //Expression<Func<People, bool>> lambda3 = lambda1.And(lambda2);
            //Expression<Func<People, bool>> lambda4 = lambda1.Or(lambda2);
            //Expression<Func<People, bool>> lambda5 = lambda1.Not();

            //List<People> peoples = new List<People>()
            //{
            //    new People(){Id=4,Name="123",Age=4},
            //    new People(){Id=5,Name="234",Age=5},
            //    new People(){Id=6,Name="345",Age=6},
            //};
            //var list0 = peoples.Where(lambda3.Compile()).ToList();
            //var list = peoples.WhereBy<People>(lambda3);//扩展与上面等价
            //var query0=peoples.AsQueryable().Where(lambda4);
            //var query = peoples.AsQueryable().WhereBy(lambda5);//扩展与上面等价
        }
        #endregion

        #region MD5 Md5公开的算法，任何语言实现后其实都一样，通用的

        //{
        //    Console.WriteLine(MD5Encrypt.Encrypt("1"));
        //    Console.WriteLine(MD5Encrypt.Encrypt("1"));
        //    Console.WriteLine(MD5Encrypt.Encrypt("123456小李"));
        //    Console.WriteLine(MD5Encrypt.Encrypt("113456小李"));
        //    Console.WriteLine(MD5Encrypt.Encrypt("113456小李113456小李113456小李113456小李113456小李113456小李113456小李"));
        //    string md5Abstract1 = MD5Encrypt.AbstractFile(@"D:\Projects\NetCore\NetCore.sln");
        //    string md5Abstract2 = MD5Encrypt.AbstractFile(@"D:\Projects\NetCore\NetCore.sln");
        //    //1 防止看到明文    
        //    //密码--md5一下--保存密文--登陆的时候--输入的密码也md5一下--比对
        //    //md5不能解密？不能解密  网上的解密都是基于样本比对
        //    //密码复杂点  加盐(密码+ruanmou 再MD5)(双MD5)

        //    //文件摘要，只有相同文件才能相同的md5
        //    //  防篡改：下载VS安装文件(MD5)--官网--软件站下载(MD5) 
        //    //急速秒传：百度云--一瞬间就传完了--第一次上传，传完保存md5--先计算md5--比对--匹配了就不需要上传
        //    // git/svn：源码管理器svn--即使断网了，文件有任何改动都能被发现--本地存了一个文件的MD5--文件有更新，就再对比下MD5

        //    //任何数据MD5后结果都不一样，到目前为止还没碰到  2的128次方
        //    //防抵赖：文章--md5(权威机构)

        //}

        #endregion

        #region Des

        //{
        //    //可逆对称加密 
        //    //数据传输   加密速度快   密钥的安全是问题
        //    string desEn = DesEncrypt.Encrypt("王殃殃");
        //    string desDe = DesEncrypt.Decrypt(desEn);
        //    string desEn1 = DesEncrypt.Encrypt("张三李四");
        //    string desDe1 = DesEncrypt.Decrypt(desEn1);
        //}

        #endregion

        #region Rsa  公开算法 即使拿到密文 你是推算不了密钥 也推算不了原文 加密钥&解密钥是一组的  

        //{
        //    //可逆非对称加密 
        //    //加密解密速度不快  安全性好
        //    KeyValuePair<string, string> encryptDecrypt = RsaEncrypt.GetKeyPair();
        //    string rsaEn1 = RsaEncrypt.Encrypt("net", encryptDecrypt.Key);//key是加密的
        //    string rsaDe1 = RsaEncrypt.Decrypt(rsaEn1, encryptDecrypt.Value);//value 解密的   不能反过来用的
        //    //加密钥  解密钥  钥匙的功能划分

        //    //公钥    私钥      公开程度划分

        //    //根据需求  可以交错的

        //    //网站：1、保密 只有你和你的目标能看懂内容
        //    //      2、完整 不可篡改
        //    //      2、可获得 信息可以拿到
        //    //CA证书：  CA机构作为中间权威机构 非营利性机构 基础信任（浏览器内置证书，全球公认无条件相信）
        //    //1、网站所属公司向CA机构申请证书
        //    //2、CA机构生成公司或网站的基础信息，公钥(解密key)和证书的数字签名
        //    //3、CA把公司的基本信息MD5摘要加密得到一个唯一值，CA自己再非对称可逆加密一下，只有CA才知道加密key,保证信息来源于CA
        //    //4、CA把解密key内置在证书里，公司网站从内置证书里通过解密key解密成功得到的密文确定来自于CA 并可看到CA加密的该公司证书信息
        //    //5、用户使用浏览器客户端(内置根证书)请求（导入了申请的CA证书的）网站， 网站发给用户一个自己的公钥(加密key)
        //    //6、用户拿到加密key加密密文传输给公司网站，公司网站使用自己的私钥(解密key)解密密文，这样只有用户发送给该公司的加密密文才可以被解开

        //}
        #endregion

        #region 多线程

        {
            //进程：计算机概念，程序在服务器运行时占据全部计算资源总合（虚拟的）
            //线程：计算机概念，进程在响应操作时最小单位，也包含cpu 内存 网络 硬盘IO(虚拟概念)
            //一个进程会包含多个线程，线程隶属于进程，进程销毁线程也没了
            //句柄：long数字，是操作系统标识应用程序
            //多线程：计算机概念，一个进程有多个线程同时运行


            //C#里面的多线程：
            //Thread类是C#语言对线程对象的一个封装

            //1、多个cpu的核可以并行工作，多个模拟线程
            //     4核8线程，这里的线程指的是模拟线程
            //2、cpu分片，1s的处理能力分成1000份，系统调度着去响应不同的任务
            //     从宏观角度来说：感觉就是多个任务在并发执行
            //     从微观角度来说，一个物理cpu同一时刻只能为一个任务服务

            //并行与并发：
            //并行：多核之间叫并行
            //并发：cpu分片的并发

            //同步异步：
            //    同步方法：发起调用，完成之后才继续下一行，非常符合逻辑开发
            //              同步方法卡界面，UI线程负责整个程序计算，耗时长且必须全部执行完毕才可以响应别的操作，无暇他顾
            //              有序进行
            //    异步方法：发起调用，不等待完成，直接进入下一行，启动一个新的线程完成方法的计算
            //              异步方法不卡界面，UI线程闲置或不让UI线程执行耗时运算，立即即可完毕，可以立即响应其它操作
            //              如：web应用发个短信通知，异步多线程去发短信 
            //              多线程其实是使用更多资源去换性能
            //              一个订单表统计很耗时间，能不能用多线程优化性能？不能！因为这就是一个操作，没法细分线程并行
            //              需要查询数据库/调用接口/读硬盘文件/做数据计算，能不能多线程优化性能？可以，多个线程任务可以并行
            //              线程不是越多越好，因为资源有限，而且调用有损耗
            //              无序启动线程，无序结束(谁先干完谁先结束)，同一个任务同一个线程，执行时间也不确定，cpu分片
            //              使用多线程请一定小心，很多事儿不是想当然的，尤其是多线程操作间有顺序要求的时候，
            //              通过延迟一点启动来控制顺序？或者预计下结束顺序？这些都不靠谱！

            //需要控制多线程顺序：Task类提供了很多api 注意core里不支持BeginInvoke

            //Action action = () =>
            //{
            //    Console.WriteLine($"委托执行******{Thread.CurrentThread.ManagedThreadId}{DateTime.Now:yyyy/MM/dd HH:mm:ss}************");
            //};
            //AsyncCallback callback = ar =>
            //    {
            //        Console.WriteLine(
            //            $"btnAsyncAdvanced_Click计算完毕，parameter:{ar.AsyncState?.ToString()}***{Thread.CurrentThread.ManagedThreadId}{DateTime.Now:yyyy/MM/dd HH:mm:ss}************");
            //    };
            //var asyncResult = action.BeginInvoke(callback, null);


            ////通过IsCompleted 等待，卡界面--主线程等待，边等待边提示
            //int i = 0;
            //while (!asyncResult.IsCompleted)
            //{
            //    if (i < 9)
            //    {
            //        Console.WriteLine($"等待{++i * 10}%******{Thread.CurrentThread.ManagedThreadId}{DateTime.Now:yyyy/MM/dd HH:mm:ss}************");
            //    }
            //    else
            //    {
            //        Console.WriteLine($"完毕{++i * 10}%******{Thread.CurrentThread.ManagedThreadId}{DateTime.Now:yyyy/MM/dd HH:mm:ss}************");
            //    }
            //    Thread.Sleep(200);
            //}

            //asyncResult.AsyncWaitHandle.WaitOne();//直接等待任务完成
            //asyncResult.AsyncWaitHandle.WaitOne(-1);//无限期等待任务完成
            //asyncResult.AsyncWaitHandle.WaitOne(5000);//最多等待5s

            //action.EndInvoke(asyncResult);//等待某次异步调用操作结束，即时等待,EndInvoke还可以获取委托的返回值

            // Console.WriteLine($"btnAsyncAdvanced_Click开始计算{Thread.CurrentThread.ManagedThreadId}{DateTime.Now:yyyy/MM/dd HH:mm:ss}************");


            // Func<int> fun = () =>
            // {
            //     Console.WriteLine($"执行***{Thread.CurrentThread.ManagedThreadId}{DateTime.Now:yyyy/MM/dd HH:mm:ss}************");
            //    return DateTime.Now.Hour;
            // };
            // int iResult = fun.Invoke();//
            // //IAsyncResult fResult = fun.BeginInvoke(ar => { }, "参数");
            //// var retunValue = fun.EndInvoke(fResult);
            // Console.WriteLine($"执行结果：{iResult}***{Thread.CurrentThread.ManagedThreadId}{DateTime.Now:yyyy/MM/dd HH:mm:ss}************");

            // //一个异步操作只能EndInvoke一次
        }
        {
            //多线程并发任务，某个失败后，希望通知别的线程，都停下来，怎么解决？
            //cts初始化有个IsCancellationRequested属性默认为false
            var cts = new CancellationTokenSource();//cts 线程安全
            TaskFactory taskFactory = new TaskFactory();
            var taskList = new List<Task>();
            for (int i = 0; i < 50; i++)
            {
                //name 是闭包里面的变量，一次循环只有一个name(新内存空间)，
                //而i是循环变量，始终只有一个内存区域，显示输出始终等于循环完毕后的值
                string name = $"btnThreadCore_Click{i}";
                Action<object> act = t =>
                {
                    try
                    {
                        //if (cts.IsCancellationRequested)
                        //{
                        //    Console.WriteLine("{0} 取消一个任务的执行", t);
                        //}
                        Thread.Sleep(2000);
                        if (t.ToString().Equals("btnThreadCore_Click11"))
                        {
                            throw new Exception($"{t} 执行失败");
                        }
                        if (t.ToString().Equals("btnThreadCore_Click12"))
                        {
                            throw new Exception($"{t} 执行失败");
                        }
                        if (cts.IsCancellationRequested)//检查信号量
                        {
                            Console.WriteLine($"{t} 放弃执行");
                            return;
                        }
                        else
                        {
                            Console.WriteLine($"{t} 执行成功");
                        }
                    }
                    catch (Exception ex)
                    {
                        cts.Cancel();
                        Console.WriteLine(ex.Message);
                    }
                };
                taskList.Add(taskFactory.StartNew(act, name, cts.Token));
            }
            Task.WaitAll(taskList.ToArray());
            //1、准备CancellationTokenSource 2、try-catch-cts.cancel 3、action要随时判断IsCancellationRequested
            //2.1、启动线程传递Token 2.2、异常抓取 

        }

        {
            //线程安全问题一般都是有全局变量/共享变量/静态变量/硬盘文件/数据库的值，只要多线程都能访问和修饰
            //最好自己做好数据分拆，避免多线程操作同一数据


            var iNumSync = 0;//线程安全
            var iNumASync = 0;//非线程安全
            var iListAsync = new List<int>();//非线程安全 因为有可能多个线程同时在一块内存空间写操作

            for (int i = 0; i < 10000; i++)
            {
                iNumSync++;
            }
            for (int i = 0; i < 10000; i++)
            {
                int k = i;
                lock (_lock)//lock锁住_lock变量的内存引用空间，确保一次只有一个线程在操作
                {
                    //Task.Run(()=>iNumASync++);
                    Task.Run(() => iListAsync.Add(k));
                }
            }
            Thread.Sleep(5 * 1000);

            Console.WriteLine($"***iNumSync={iNumSync},iNumASync={iNumASync};{Thread.CurrentThread.ManagedThreadId}{DateTime.Now:yyyy/MM/dd HH:mm:ss}************");

            //线程安全集合
            //System.Collections.Concurrent.ConcurrentQueue<int>

        }
        {
            //await/async 

        }

        #endregion

        #region CLR内存回收
        {
            //托管资源垃圾回收--CLR提供GC
            //1、什么样的对象需要垃圾回收？托管资源+引用类型

            //托管资源和非托管资源
            //托管资源就是CLR控制的，new的对象，string字符串，变量
            //非托管资源不是CLR控制的，数据库连接，文件流，句柄，打印机连接；
            //Using(SqlConnection)被C#封装了管理哪个非托管的数据库连接资源，
            //只要是手动释放的，都是非托管的
            //2、哪些对象的内存，能被GC回收？对象访问不到了，那就可以被回收了(脱离作用域大括号后访问不到的对象)
            //3、静态变量在内存中唯一并且不会被释放的，它的生命周期是应用程序级，
            //例如web项目，只有网站重启才会被回收 
            //静态变量中的引用类型都是不被回收的，总之离开作用域后不能访问的都被回收。
            //4、对象是如何分配到堆上的？什么时候发生GC？
            //连续分配在堆上，每次new对象分配空间先检查空间够不够，如果不够就会发生GC
            //a)new 对象时--临界点
            //b)GC.Collect() 强制GC 
            //c)程序退出时会GC
            //GC.Collect();//频繁GC是不好的，GC是全局的
            //项目中有6个小时才运行new一次，什么时候GC？不GC，可以手动GC
            //5、GC的过程是怎样的？
            //N个对象--全部对象标记为垃圾--入口开始遍历--访问到的就标记(+1)
            //--遍历完就清理内存--产生不连续内存--压缩--地址移动--修改变量指针地址(这是全局阻塞)--完毕后正常
            //清理内存分2种情况：
            //a)无析构函数，直接清理内存
            //b)把对象转移到一个的单独的队列，会有个析构器线程专门做这个，
            //通常在析构函数内部用来做非托管资源释放，因为CLR肯定调用，所以避免使用者忘记的情况
            //6、垃圾回收策略
            //对象分代：3代
            //0代：对象第一次分配到堆就是0代对象
            //1代：经历了第一次GC之后依然还在内存的对象
            //2代：经历了第二次或以上GC仍驻内存的对象
            //垃圾回收时，优先回收0代对象，提升效率，再依次去回收1，2代对象--再不够就真内存不够了5555
            //大对象堆策略--内存移动大对象；0代空间问题；所以 (80000字节以上都叫大对象，没有分代就直接2代)

            //析构函数&Dispose
            //析构函数：被动清理
            //Dispose：CLR主动清理

        }
        #endregion

        #region 缓存

        //打开一个网页的过程：
        //浏览器--请求--服务器--处理请求发响应--浏览器展示
        //Http协议：数据传输的格式
        //信息是否缓存：一定是服务器控制的
        //ResponseHeader--Cache-Control 来指定下缓存策略
        //1、客户端缓存：缩短网络路劲，加快响应速度；减少请求，降低服务器压力
        //客户端缓存只影响当前用户
        //2、DNS缓存：互联网的第一跳，DNS缓存就是CDN，内容分发网络（CDN加速就是CDN缓存）：cdn缓存影响的是一批用户
        //CDN的要点：将请求路由到就近的CDN节点，以提高响应速度。
        //你的域名dns指向到提供cdn加速的服务商做解析？
        //反向代理：通过服务器访问外网，翻墙， 反过来:同样缩短网络路劲，加快响应速度；减少请求，降低服务器压力：针对全部用户
        //1、隔离网络2、网络加速，反向代理双网卡(电信，联通)3、负载均衡4、缓存(跟CDN，也是识别一下header，压缩到一个物理路径/内存)
        //


        //1、removeAll
        //2、添加缓存时，key带上规则，比如包含_menu_ 清理时就只删除指定keys的缓存数据

        //过期策略：1、永久有效2、过期时间3、滑动过期

        //多线程缓存
        //{
        //    List<Task> taskList=new List<Task>();
        //    for (int i = 0; i < 1000; i++)
        //    {
        //        int k = i;
        //        taskList.Add(Task.Run(()=>CustomCache.Add($"TestKey_{k}",$"TestValue_{k}",10)));
        //        Task.WaitAll(taskList.ToArray());
        //    }
        //}

        //数据集合问题
        //1、数组 内存中连续存储 节约空间 可以索引访问 读取快，增删慢
        //2、链表 内存中非连续摆放，存储数据+地址，只能顺序查找数据，读取慢，增删快
        //3、队列 Queue 先进先出，放任务延迟执行，A不断写入日志任务，B不断取任务去执行

        //IEnumerable 任何数据集合都实现了它，为不同的数据结构提供了统一的访问方式 迭代器模式
        //IQueryable  表达式目录树与IQueryableProvider

        //分布式缓存：
        //MemeryCashed 最早流行
        //Nosql-Redis 主流方案
        //Nosql:非关系型数据库，Not，Only，Sql
        //数据库的关系复杂：好友关系（qq好友100个，映射表，数据冗余，关系型数据库开始累赘）
        //所以就产生了NoSql了
        //特点：基于内存：
        //没有严格的数据格式，不是一行数据的列必须一样
        //丰富的类型，满足web2.0的需求
        //Redis：Remote Dictionary Server 远程字典服务器 基于内存管理(数据存在内存)
        //实现了5种数据结构(分别对应各种需求)，单线程模型的应用程序，对外提供插入-查询-固化-集群功能
        //RDM：可视化操作Redis数据的管理工具(SqlClient)
        //基于内存管理;速度快，Redis还有个固化数据的功能，。VirtualMemory，把一些不经常访问存放在硬盘
        //AOF：数据变化的记录日志，很少用
        //Redis毕竟不是数据库，只能用来提升性能，不能作为数据库
        //单线程模型：nodejs单线程，整个进程只有一个线程，线程就是执行流，性能低？实际上并非如此
        //一次网络请求操作==正则解析请求+计算+数据库操作(发命令--等结果)，单线程都是用事件驱动，发起命令就做下一件事儿，这个线程是完全不做等待的，一直在计算，单线程非常高。
        //单线程多进程的模式来提供集群服务
        //单线程最大的好处就是原子性操作（事务性）
        //ServiceStack(1小时3600次请求--可破解)---意味着net的Ado.Net 其实更像ORM
        //StackExchange(免费) 封装了连接+命令 更像ORM
        //Redis 5大数据结构
        //1、String key-value 缓存 value不超过512MB
        //2、Hash  多个key-string类型的value最小是512byte，即使只保存一个1，也要占用512byte空间
        // 而hash是zipmap存储的，紧密排列
        //3、Set 去重：IP统计去重：添加好友申请，投票限制，点赞，共同好友，推荐好友
        //4、ZSet 去重   而且自带排序   排行榜/统计全局排行榜
        //5、List 

        //ServiceStack://破解,找到3600这行代码，把它干掉
        //SetAll&&AppendToValue&&GetValue&&GetAndSetValue&&IncrementValueBy,这些看上去是组合命令，但实际上是一个具体的命令，原子性命令(事务性)
        //使用Redis能够拦截无效的请求，如果这一层没有，所有的请求压力都到数据库
        //Redis为开发而生，为各种需求提供解决方案。
        //
        #endregion

        #region webApi

        //跨域请求：浏览器请求时，如果A网站(域名+端口号)页面里面，通过XHR请求B域名，这个就是跨域
        //这个请求是可以正常到达B网站服务器后端，正常相应(200)，但是浏览器不允许这样操作，除非
        //有声明(Access-Control-Allow-Origin),
        //浏览器同源策略：处于安全考虑，浏览器限制脚本去发起跨站请求，但是，页面是js/css/图片/iframe 
        //这些是浏览器自己发起的，可以跨域 (可以)
        //JSONP:脚本标签自动请求--请求回来的内容执行个回调方法--解析数据
        //CORS:跨域资源共享，允许服务器在相应头里指定Access-Control-Allow-Origin,浏览器按照相应来操作
        //[EnableCores(origins:"http://localhost:8090/",header:"")]

        #endregion

        #region 数据库
        //数据库：把东西有序放好还能随时找到的工具 它是一个应用程序，数据固化在硬盘上
        //sqlserver自增int，默认聚集索引，一个表聚集索引只有一个(默认主键)，但是可含多个字段
        //非聚集索引：不影响数据的物理排序，但是重复存储一个数据和位置 一张表可以有多个非聚集索引
        //找数据：先找索引--快速定位--拿到数据
        //优化：
        //1、避免对列的计算，任何形式都要避免
        //2、in，or查询，索引会失效，可能是拆分
        //3、in 换 exists
        //4、not in 不要用，不走索引 is null 和 is not null <>都不走索引 可以拆分为< 和>

        #endregion

        #region esClient

        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
            .DefaultIndex("test");//需要提前在es里先新建test索引

            #region 集群连接方式
            //var uris = new[]
            //        {
            //   	new Uri("http://localhost:9200"),
            //  new Uri("http://localhost:9201"),
            //  new Uri("http://localhost:9202"),
            //        };
            //var connectionPool = new SniffingConnectionPool(uris);
            //var settings = new ConnectionSettings(connectionPool)
            //	.DefaultIndex("people"); 
            //var client = new ElasticClient(settings);
            #endregion

            var client = new ElasticClient(settings);

            #region 使用linq查询

            //// 数据刷盘延迟--默认1s 
            //var searchResponse = client.Search<Person>(s => s
            //    .From(0)
            //    .Size(10)
            //    .Query(q => q
            //        .Match(m => m
            //            .Field(f => f.FirstName)
            //            .Query("张三")
            //        )
            //    )
            //);
            //var people = searchResponse.Documents;
            //Console.WriteLine("查询结果");
            //foreach (var item in people)
            //{
            //    Console.WriteLine($"id:{item.Id},firstname:{item.FirstName},lastname:{item.LastName}");
            //}

            //Console.WriteLine("**********");
            //// select * from tabel where name="1" and age>1

            ////{ "query" : { "bool" : { "must": [{ "match_all" : { } }]} },"from" : 0,"size" : 1}
            ////{"query" : {"bool" : {"must" : [{"match" : {"name" : {"query" : "1", "type" : "phrase"}}},{"range" : {"age" : {"gt" : "1"}}}]}},"from" : 0,"size" : 1}

            //var ss = client.Search<Person>(s => s.Query(
            //        m => m.Bool(
            //            m => m.Must(
            //                x => x.Match(m => m.Field(f => f.FirstName).Query("1")
            //                ), mm => mm.Range(xx => xx.Field(f => f.Id).GreaterThan(1))
            //            )
            //        )

            //    )
            //).Documents;


            //var searchResponse = client.Search<Person>(s => s
            //	.From(0)
            //	.Size(10)
            //	.Query(q => q
            //		 .Match(m => m
            //			.Field(f => f.FirstName)
            //			.Query("Martijn1")
            //		 )
            //	)
            //);
            //var people = searchResponse.Documents;
            //Console.WriteLine("查询结果");
            //foreach (var item in people)
            //{
            //	Console.WriteLine($"id:{item.Id},firstname:{item.FirstName},lastname:{item.LastName}");
            //}
            #endregion

            #region 使用默认sql-cli 查询

            {
                //var result = await client.Sql.QueryAsync(q => q.Query("select id form test"));
                //var dataTable = new DataTable();
                //dataTable.Columns.AddRange(result.Columns.Select(m => new DataColumn(m.Name, ReflectTypeHelper.GetTypeByEsTypeString(m.Type))).ToArray());
                //foreach (var resultRow in result.Rows)
                //{
                //    var row = dataTable.NewRow();
                //    for (var columIndex = 0; columIndex < result.Columns.Count; columIndex++)
                //    {
                //        row[columIndex] = Convert.ChangeType(resultRow[columIndex].As<object>(), dataTable.Columns[columIndex].DataType);
                //        //Console.WriteLine($"Name:{colums[columIndex].Name} Type:{colums[columIndex].Type}_{dataTable.Columns[columIndex].DataType} value:{row[columIndex]}");
                //    }
                //    dataTable.Rows.Add(row);
                //}
                //var json = JsonConvert.SerializeObject(dataTable);
            }
            #endregion
        }

        #endregion

        #region EF6 

        //LinqToSql包含对象的CRUD操作的API Mapping 并不是ORM就不能写sql
        //dapper  
        //IBatis.Net
        //sugar 
        //sql--反射生成sql--自动执行--反射绑定结果
        //缓存(占内存)
        //开发快捷，降低学习成本
        //sql固定生成，但是僵化，对索引引用不够好，分页算法就不够好，在复杂情况下不好应付
        //ORM工具一般也可以支持写SQL 
        //ORM一般还能适应不同数据库的迁移
        //ORM只是一个工具，去完成它擅长的事儿

        //ORM(对象关系映射):可以理解为一个封装/代理/面向对象的方式来操作数据，会把整个数据库搬到程序里面来
        //封装-->映射  通过这些类就实现对数据的操作    目前EntityFramwork6 支持多种数据，函数，存储过程等
        //跟vs项目完美结合
        //codeFirst:CodeFistFromDB

        try
        {

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw new Exception(e.Message);
        }

        #endregion





    }


    //推荐锁 私有 静态 只读 并且是一个Object对象
    //不能为null string，因为null没有内存空间，报错，string在内存分配上是重用的，任意两个对象同样的内容会指向同一块内存
    //不推荐lock(this) //this 锁的是当前类型实例 如果是类中方法，外部锁定一个实例对象，会导致锁住同一个实例，进行排队
    //lock里面的代码不要太多，这里面是单线程的。
    private static readonly object _lock = new object();

    private static int Count = 0;

    /// <summary>
    /// 死锁？
    /// </summary>
    private void DoTest()
    {
        //lock (_lock)
        //{
        //    Task.Delay(1001).Wait();
        //}

        //Console.WriteLine($"延迟{Count++}s:{DateTime.Now.Second}");
        //if (Count < 10)
        //{
        //    DoTest();
        //}
        lock (this)//单线程不会死锁,因为this变量区域被单线程这一个线程占据，但会死循环，多线程就死锁
        {
            Count++;
            if (DateTime.Now.Day < 28 && Count < 10)
            {
                Console.WriteLine($"第{Count++}次调用自己{DateTime.Now.Day}");
                this.DoTest();
            }
            else
            {
                Console.WriteLine($"第{Count++}次调用自己：28号课程结束{DateTime.Now.Day}");
            }
        }
    }


    public static async Task TestHello()
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:5001");
        var client = new Greeter.GreeterClient(channel);
        //client.SayHello()
        var reply = await client.SayHelloAsync(new HelloRequest
        {
            Name = "〤流浪＜少年℃"
        });
        Console.WriteLine($"GreeTer 服务器返回数据：{reply.Message}");
    }

    private static async Task TestMath()
    {
        #region token
        //string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiRWxldmVuIiwiRU1haWwiOiI1NzI2NTE3N0BxcS5jb20iLCJBY2NvdW50IjoieHV5YW5nQHpoYW94aUVkdS5OZXQiLCJBZ2UiOiIzMyIsIklkIjoiMTIzIiwiTW9iaWxlIjoiMTg2NjQ4NzY2NzEiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsIlNleCI6IjEiLCJuYmYiOjE1OTA1OTQ1OTEsImV4cCI6MTU5MDU5ODEzMSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1NzI2IiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1NzI2In0.MGXKLQ9ZVh0xvsQ1kNhb5gXi_8hqD2RL8metxhjEFiU";
        //var headers = new Metadata { { "Authorization", $"Bearer {token}" } };
        #endregion

        using var channel = GrpcChannel.ForAddress("https://localhost:5001");
        var client = new CustomMath.CustomMathClient(channel);
        //var invoker = channel.Intercept(new CustomClientInterceptor());

        Console.WriteLine("***************单次调用************");
        {
            var reply = await client.SayHelloAsync(new HelloRequestMath { Name = "Eleven" });
            Console.WriteLine($"CustomMath {Thread.CurrentThread.ManagedThreadId} 服务返回数据:{reply.Message} ");
        }
        Console.WriteLine("***********************单次调用异步********************************");
        {
            RequestPara requestPara = new RequestPara() { ILeft = 123, IRight = 234 };

            var replyPlus = await client.PlusAsync(requestPara);
            Console.WriteLine($"CustomMath {Thread.CurrentThread.ManagedThreadId}  服务返回数据:{replyPlus.Result}  Massage={replyPlus.Message}");
        }
        Console.WriteLine("***********************单次调用同步********************************");
        {
            var replyPlusSync = client.Plus(new RequestPara() { ILeft = 123, IRight = 234 });
            Console.WriteLine($"CustomMath {Thread.CurrentThread.ManagedThreadId}  服务返回数据:{replyPlusSync.Result}  Massage={replyPlusSync.Message}");
        }
        //Console.WriteLine("***********************单次调用异步带Token********************************");
        //{
        //    RequestPara requestPara = new RequestPara() { ILeft = 123, IRight = 234 };

        //    var replyPlus = await client.PlusAsync(requestPara, headers);
        //    Console.WriteLine($"CustomMath {Thread.CurrentThread.ManagedThreadId}  服务返回数据:{replyPlus.Result}  Massage={replyPlus.Message}");
        //}

        //Console.WriteLine("***********************单次调用同步带Token********************************");
        //{
        //    RequestPara requestPara = new RequestPara() { ILeft = 123, IRight = 234 };
        //    var replyPlusSync = client.Plus(requestPara, headers);
        //    Console.WriteLine($"CustomMath {Thread.CurrentThread.ManagedThreadId}  服务返回数据:{replyPlusSync.Result}  Massage={replyPlusSync.Message}");
        //}

        //Console.WriteLine("**************************空参数*****************************");
        //{
        //    var countResult = await client.CountAsync(new Empty());
        //    Console.WriteLine($"随机一下 {countResult.Count}");
        //    var rand = new Random(DateTime.Now.Millisecond);
        //}
        Console.WriteLine("**************************客户端流*****************************");
        {
            var bathCat = client.SelfIncreaseClient();
            for (int i = 0; i < 10; i++)
            {
                await bathCat.RequestStream.WriteAsync(new BathTheCatReq() { Id = new Random().Next(0, 20) });
                await Task.Delay(100);
                Console.WriteLine($"This is {i} Request {Thread.CurrentThread.ManagedThreadId}");
            }
            Console.WriteLine("**********************************");
            //发送完毕
            await bathCat.RequestStream.CompleteAsync();
            Console.WriteLine("客户端已发送完10个id");
            Console.WriteLine("接收结果：");

            foreach (var item in bathCat.ResponseAsync.Result.Number)
            {
                Console.WriteLine($"This is {item} Result");
            }
            Console.WriteLine("**********************************");
        }
        //Console.WriteLine("**************************服务端流*****************************");
        //{
        //    IntArrayModel intArrayModel = new IntArrayModel();
        //    for (int i = 0; i < 15; i++)
        //    {
        //        intArrayModel.Number.Add(i);//Number不能直接赋值，
        //    }

        //    CancellationTokenSource cts = new CancellationTokenSource();
        //    cts.CancelAfter(TimeSpan.FromSeconds(5.5)); //指定在2.5s后进行取消操作
        //    var bathCat = client.SelfIncreaseServer(intArrayModel, cancellationToken: cts.Token);

        //    //var bathCat = client.SelfIncreaseServer(intArrayModel);//不带取消
        //    var bathCatRespTask = Task.Run(async () =>
        //    {
        //        await foreach (var resp in bathCat.ResponseStream.ReadAllAsync())
        //        {
        //            Console.WriteLine(resp.Message);
        //            Console.WriteLine($"This is  Response {Thread.CurrentThread.ManagedThreadId}");
        //            Console.WriteLine("**********************************");
        //        }
        //    });
        //    Console.WriteLine("客户端已发送完10个id");
        //    //开始接收响应
        //    await bathCatRespTask;
        //}
        Console.WriteLine("**************************双流*****************************");
        {
            var bathCat = client.SelfIncreaseDouble();
            var bathCatRespTask = Task.Run(async () =>
            {
                await foreach (var resp in bathCat.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine(resp.Message);
                    Console.WriteLine($"This is  Response {Thread.CurrentThread.ManagedThreadId}");
                    Console.WriteLine("**********************************");
                }
            });
            for (int i = 0; i < 10; i++)
            {
                await bathCat.RequestStream.WriteAsync(new BathTheCatReq() { Id = new Random().Next(0, 20) });
                await Task.Delay(100);
                Console.WriteLine($"This is {i} Request {Thread.CurrentThread.ManagedThreadId}");
                Console.WriteLine("**********************************");
            }
            //发送完毕
            await bathCat.RequestStream.CompleteAsync();
            Console.WriteLine("客户端已发送完10个id");
            Console.WriteLine("接收结果：");
            //开始接收响应
            await bathCatRespTask;
        }

        Console.WriteLine("**************************双流+取消*****************************");
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(2.5)); //指定在2.5s后进行取消操作
            var bathCat = client.SelfIncreaseDouble(cancellationToken: cts.Token);
            var bathCatRespTask = Task.Run(async () =>
            {
                await foreach (var resp in bathCat.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine(resp.Message);
                    Console.WriteLine($"This is  Response {Thread.CurrentThread.ManagedThreadId}");
                    Console.WriteLine("**********************************");
                }
            });
            for (int i = 0; i < 10; i++)
            {
                await bathCat.RequestStream.WriteAsync(new BathTheCatReq() { Id = new Random().Next(0, 20) });
                await Task.Delay(100);
                Console.WriteLine($"This is {i} Request {Thread.CurrentThread.ManagedThreadId}");
                Console.WriteLine("**********************************");
            }
            //发送完毕
            await bathCat.RequestStream.CompleteAsync();
            Console.WriteLine("客户端已发送完10个id");
            Console.WriteLine("接收结果：");
            //开始接收响应
            await bathCatRespTask;
        }
    }

    /// <summary>
    /// 封装请求
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static async Task<bool> ClientPostWithRequestMessage(PostParameterDto input)
    {
        //var content = new StringContent(TextJsonConvert.SerializeObject(input), Encoding.UTF8);

        var client = HttpClientPool.Instance();
        //client.DefaultRequestVersion = HttpVersion.Version11;
        //client.BaseAddress = new Uri($@"{input.Host}:{input.Port}");
        //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
        //client.DefaultRequestHeaders.Add("content-type", "application/json");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {input.AccessToken}");
        bool success;

        try
        {
            #region content header
            var request = new HttpRequestMessage(HttpMethod.Post, input.FullUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", input.AccessToken);
            request.Content = input.Content;
            //request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //request.Headers.Add("Authorization", $"Bearer {input.AccessToken}");
            #endregion
            Console.WriteLine($"开始请求:{input.FullUrl}");

            var result = new HttpResponseMessage();
            if (input.ReturnVoid)
            {
                await client.SendAsync(request);
            }
            else
            {
                //result = await client.PostAsync(input.GetUrl, input.Content);
                await client.SendAsync(request).ContinueWith(responseTask =>
                {
                    Console.WriteLine("Response: {0}", responseTask.Result);
                });
            }
            if (result.IsSuccessStatusCode)
            {
                Console.WriteLine($"请求成功！");
                if (!input.ReturnVoid)
                {
                    if (input.ReturnObj)
                    {
                        var obj = result.Content.As<object>();
                        Console.WriteLine($"请求结果:{obj}");
                    }
                    else
                    {
                        var msg = await result.Content?.ReadAsStringAsync();
                        Console.WriteLine($"{input.FullUrl}请求结果:{msg}");
                    }
                }
            }
            Console.WriteLine($"========================================================================================================");
            Console.WriteLine($"{await result.Content?.ReadAsStringAsync()}");
            Console.WriteLine($"========================================================================================================");
            success = result.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"请求失败：{ex.Message}");
            success = false;
        }
        return success;
    }


    /// <summary>
    /// 封装请求
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static async Task<bool> ClientPost(PostParameterDto input)
    {
        using var client = new HttpClient(); //HttpClientPool.Instance();

        var fullUrl = RequestExtends(client, input.AccessToken, input.FullUrl);
        ContentExtends(input);
        bool success;
        try
        {
            var result = new HttpResponseMessage();
            if (input.ReturnVoid)
            {
                await client.PostAsync(fullUrl, input.Content);
            }
            else
            {
                result = await client.PostAsync(fullUrl, input.Content);
            }
            if (result.IsSuccessStatusCode)
            {

                Console.WriteLine($"请求成功！");
                if (!input.ReturnVoid)
                {
                    if (input.ReturnObj)
                    {
                        var obj = result.Content.As<object>();
                        Console.WriteLine($"请求结果:\n{obj}");
                    }
                    else
                    {
                        var msg = await result.Content.ReadAsStringAsync();
                        Console.WriteLine($"请求{fullUrl}结果:\n{Newtonsoft.Json.JsonConvert.DeserializeObject<object>(msg)}");

                        #region 厦门会展

                        #region 一标段

                        //考勤历史记录
                        //var returnObj = JsonConvert.DeserializeObject<SectionOneGateReturnData>(msg);
                        //var gates = returnObj.Data.Data.Select(m => new GateCommon(Guid.NewGuid())
                        //{
                        //    SerialNo = m.SerialNumber,
                        //    ProjectSysNo = m.ProjectSysNo,
                        //    SubContractorSysNo = m.SubContractorSysNo,
                        //    SubContractorName = m.SubContractorName,
                        //    TeamSysNo = m.TeamSysNo,
                        //    TeamName = m.TeamName,
                        //    WorkTypeName = m.TeamName,
                        //    WorkerNo = m.WorkerSysNo,
                        //    PersonnelId = m.WorkerSysNo,
                        //    PersonnelName = m.WorkerName,
                        //    IdCard = m.IDCardNumber,
                        //    InOutType = m.Type.ToString(),
                        //    Gender = m.Gender.ToString(),
                        //    Photo = m.Image,
                        //    PhotoUrl = m.Image,
                        //    CheckChannel = m.CheckChannel,
                        //    FaceSimilarity = m.FaceSimilarity,
                        //    RecordTime = m.InDate ?? m.Time
                        //}).ToList();
                        //Console.ReadKey();

                        #endregion

                        #region 二标段

                        //奥恩斯环境
                        //var returnObj = JsonConvert.DeserializeObject<SectionTwoEnvironmentReturnData>(msg);
                        //var eCommon = new EnvironmentCommon
                        //{
                        //    SerialNo = returnObj.Content.DeviceId,
                        //    Pm10 = returnObj.Content.RealtimeData.Find(m=>m.Sensor== "a34002")?.Data,
                        //    Pm10Flag = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a34002")?.Flag,
                        //    Pm25 = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a34004")?.Data,
                        //    Pm25Flag = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a34004")?.Flag,
                        //    WindSpeed = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a01007")?.Data,
                        //    WindSpeedFlag = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a01007")?.Flag,
                        //    Temperature = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a01001")?.Data,
                        //    TemperatureFlag = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a01001")?.Flag,
                        //    Noise = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a50001")?.Data,
                        //    NoiseFlag = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a50001")?.Flag,
                        //    Humidity = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a01002")?.Data,
                        //    HumidityFlag = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a01002")?.Flag,
                        //    WindDirection = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a01008")?.Data,
                        //    WdFlag = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a01008")?.Flag,
                        //    RecordTime = returnObj.Content.DateTime
                        //};
                        //Console.ReadKey();

                        #endregion

                        #endregion

                        #region 沙区管网

                        //沙区管网
                        //var returnObj = JsonConvert.DeserializeObject<List<DeviceInfoDto>>(msg);
                        //var returnObj = JsonConvert.DeserializeObject<List<DeviceHistoryDto>>(msg);
                        //var returnObj = JsonConvert.DeserializeObject<List<DeviceLocationDto>>(msg);

                        #endregion
                    }
                }
            }
            else
            {
                Console.WriteLine($"========================================================================================================");
                Console.WriteLine($"{await result.Content.ReadAsStringAsync()}");
                Console.WriteLine($"========================================================================================================");
            }
            success = result.IsSuccessStatusCode;
            result.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"请求失败：\n{ex.Message}");
            success = false;
        }
        return success;
    }

    /// <summary>
    /// 封装Get请求
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static async Task<bool> ClientGet(PostParameterDto input)
    {
        using var client = new HttpClient(); //HttpClientPool.Instance();
        var fullUrl = RequestExtends(client, input.AccessToken, input.FullUrl);
        ContentExtends(input);

        //var parameterString = UrlEncoder.Default.Encode(input.Content.ReadAsStringAsync().Result);
        ////Console.WriteLine(parameterString);
        //var parameterString = System.Web.HttpUtility.UrlEncode(input.Content.ReadAsByteArrayAsync().Result);//这是个json字符串对象并不是url字符串对象
        //Console.WriteLine($"UrlEncode:{parameterString}");
        //Console.WriteLine($"UrlDecode:{System.Web.HttpUtility.UrlDecode(parameterString)}");
        ////Console.WriteLine($"{Uri.EscapeDataString(parameterString)}");

        //var result = await client.GetAsync($"{input.GetUrl}?{parameterString}");

        var result = await client.GetAsync($"{fullUrl}");
        var msg = await result.Content.ReadAsStringAsync();
        Console.WriteLine($"请求结果:{msg}");

        #region 厦门会展中心

        #region 一标段

        //环境
        //var returnObj = JsonConvert.DeserializeObject<SectionOneEnvironmentReturnData>(msg);
        ////var dateTime = DateTime.ParseExact(returnObj.Data.Data.DataTime, "yyyyMMddHHmmss", CultureInfo.CurrentCulture);
        ////Console.WriteLine($"{dateTime}");
        //var eCommon=new EnvironmentCommon
        //{
        //    SerialNo = returnObj.Data.DeviceId,
        //    Pm10 = returnObj.Data.Data.Pm10Rtd,
        //    Pm10Flag = returnObj.Data.Data.Pm10Flag,
        //    Pm25 = returnObj.Data.Data.Pm25Rtd,
        //    Pm25Flag = returnObj.Data.Data.Pm25Flag,
        //    WindSpeed = returnObj.Data.Data.WsRtd,
        //    WindSpeedFlag = returnObj.Data.Data.WsFlag,
        //    Temperature = returnObj.Data.Data.TemRtd,
        //    TemperatureFlag = returnObj.Data.Data.TemFlag,
        //    Noise = returnObj.Data.Data.B03Rtd,
        //    NoiseFlag = returnObj.Data.Data.B03Flag,
        //    Humidity = returnObj.Data.Data.RhRtd,
        //    HumidityFlag = returnObj.Data.Data.RhFlag,
        //    WindDirection = returnObj.Data.Data.WdRtd,
        //    WdFlag = returnObj.Data.Data.WdFlag,
        //    Pressure = returnObj.Data.Data.PaRtd,
        //    PaFlag = returnObj.Data.Data.PaFlag,
        //    Tsp = returnObj.Data.Data.TspRtd,
        //    TspFlag = returnObj.Data.Data.TspFlag,
        //    RecordTime = returnObj.Data.Data.DateTimeFormat
        //};
        //Console.ReadKey();

        #endregion

        #region 二标段

        //工人考勤打卡记录
        //var returnObj = JsonConvert.DeserializeObject<SectionTwoGateReturnData>(msg);
        //var gates = returnObj.Data.Select(m => new GateCommon(Guid.NewGuid())
        //{
        //    SerialNo = m.DeviceCode,
        //    ProjectSysNo = m.ProjectWorkerId?.ToString(),
        //    SubContractorSysNo = m.CompanyId?.ToString(),
        //    SubContractorName = m.CompanyName,
        //    CompanyId = m.CompanyId?.ToString(),
        //    CompanyName = m.CompanyName,
        //    TeamSysNo = m.TeamId?.ToString(),
        //    TeamName = m.TeamName,
        //    WorkTypeName = m.WorkTypeName,
        //    WorkTypeCode = m.WorkTypeCode,
        //    WorkerNo = m.WorkerId?.ToString(),
        //    PersonnelId = m.Id.ToString(),
        //    PersonnelName = m.WorkerName,
        //    IdCard = m.Identification,
        //    InOutType = m.InOutType,
        //    //Gender = m.Gender.ToString(),
        //    Photo = m.ScanPhoto,
        //    PhotoUrl = m.ScanPhoto,
        //    CheckChannel = m.ClockType,
        //    RecordTime =  m.Date
        //}).ToList();
        //Console.ReadKey();

        #endregion

        #endregion



        if (result.IsSuccessStatusCode)
        {
            Console.WriteLine($"{input.FullUrl}请求成功！");
        }
        Console.WriteLine($"========================================================================================================");
        Console.WriteLine($"                                                                                                        ");
        Console.WriteLine($"========================================================================================================");
        input.Response = result;
        return result.IsSuccessStatusCode;
    }

    /// <summary>
    /// client request 扩展设置
    /// </summary>
    /// <param name="client">HttpClient</param>
    /// <param name="token">token</param>
    /// <param name="receiveUrl">请求url</param>
    /// <returns></returns>
    private static string RequestExtends(HttpClient client, string token = null,
        string receiveUrl = null)
    {
        #region 重庆建委V1.0加密信息

        //var _keySecret = new
        //{
        //    SupplierKeyId = "ea5cbc75-1f18-4c92-a449-023f6de0f002",
        //    SupplierKeySecret = "igNd2gONoxWavfs4jfXR6SWcU8x6CdFnPep7",
        //    ProjectKeyId = "f0f50da3-3cc7-49aa-950f-5788e61dc36c",
        //    ProjectKeySecret = "ANQG5wj7S4kZXN0FhfNy9nxbGUQmckfw8ybZ"
        //};

        //var rCode = Utilities.GenerateRandomString(15);
        //var ts = DateTimeExtensions.GetUnixTime();
        //var keyId = $"{_keySecret?.SupplierKeyId}_{_keySecret?.ProjectKeyId}";
        //var signature = Utilities.GetTokenBySupplierAndProject(rCode, ts, _keySecret?.SupplierKeySecret,
        //    _keySecret?.ProjectKeySecret);

        ////Console.WriteLine($"[重庆建委采集设备][{serialNo}]请求同步数据加密信息:");
        //client.DefaultRequestHeaders.Add("keyId", keyId);
        //client.DefaultRequestHeaders.Add("ts", ts.ToString());
        //client.DefaultRequestHeaders.Add("rCode", rCode);
        //client.DefaultRequestHeaders.Add("signature", signature);
        //Console.WriteLine($"keyId:{keyId}");
        //Console.WriteLine($"ts:{ts}");
        //Console.WriteLine($"random:{rCode}");
        //Console.WriteLine($"signature:{signature}");
        #endregion

        #region 重庆V2版本需要设置 DefaultRequestHeaders

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        Console.WriteLine($"开始请求:{receiveUrl}");
        Console.WriteLine($"请求header:Authorization:Bearer {token}");
        //return receiveUrl;
        #endregion

        #region 四川省厅需要设置 Url 带上?access_token

        //return $"{receiveUrl}?access_token={token}";

        #endregion

        #region 厦门会展一标段

        //client.DefaultRequestHeaders.Add("Cookie", $"satoken=f9f4183d-5b7b-4c9b-bda7-2401def00833");

        #endregion

        #region 其它设置

        //client.DefaultRequestVersion = HttpVersion.Version11;
        //client.BaseAddress = new Uri($@"https://");
        //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
        //client.DefaultRequestHeaders.Add("content-type", "application/json");

        return receiveUrl;
        #endregion

    }

    private static PostParameterDto ContentExtends(PostParameterDto input)
    {
        #region content header

        //var json = TextJsonConvert.SerializeObject(input);
        //Console.WriteLine($"json字符串:{json}");
        //var setting = new JsonSerializerSettings
        //{
        //    ContractResolver = new CamelCasePropertyNamesContractResolver(), //序列化时key为驼峰样式
        //    DateTimeZoneHandling = DateTimeZoneHandling.Local,
        //    NullValueHandling = NullValueHandling.Ignore,
        //    DateFormatString = "yyyy/MM/dd HH:mm:ss",
        //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore//忽略循环引用
        //};

        Console.WriteLine($"序列化input对象:");//TextJsonConvert在序列化input.Content=null存在各种bug
                                          //Console.WriteLine($"input：{Newtonsoft.Json.JsonConvert.DeserializeObject(TextJsonConvert.SerializeObject(input))}");
        Console.WriteLine($"input：{JsonConvert.DeserializeObject(JsonConvert.SerializeObject(input))}");

        //input.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //input.Content.Headers.Add("ContentType", $"application/json");
        //input.Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
        //input.Content.Headers.Add("ContentType", $"multipart/form-data;charset=UTF-8");
        //input.Content.Headers.Add("ContentType", "application/x-www-form-urlencoded");

        #region cdceg添加header  已经在input.Content里添加好了
        //var tokenResult = await HttpClientPool.GetAccessToken(devKey, $@"{host}:{port}", tokenApi);
        //content.Headers.Add("AccessToken", tokenResult.AccessToken);
        #endregion

        #endregion

        return input;
    }

}
