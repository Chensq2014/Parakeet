using ConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Compute
{
    public static class SnpCompute
    {
        /// <summary>
        /// 计算SNP
        /// </summary>
        public static void ReadSnpData()
        {
            #region 直接将variant.ini文件的第一列数据拆分为两列保存到variant.csv文件中

            //// 指定variant.ini文件的路径  
            //string filePath = @"D:/mendia/data/diabetes/2443.gwas.imputed_v3.both_sexes/variant.ini";

            //// 按行读取variant.ini文件  

            //string csvFilePath = @"D:/mendia/data/diabetes/2443.gwas.imputed_v3.both_sexes/variant.csv";

            //using (StreamWriter sw = new StreamWriter(csvFilePath))
            //{
            //    using (StreamReader reader = new StreamReader(filePath))
            //    {
            //        string headerString = string.Format("{0},{1}", "chromosomes", "name");
            //        sw.WriteLine(headerString);
            //        string line;
            //        while ((line = reader.ReadLine()) != null)
            //        {
            //            // 处理每一行
            //            Console.WriteLine(line);
            //            var items = line.Split(':');
            //            var chromosome = items[0];
            //            var name = items[1];
            //            //dt.Rows.Add(chromosome, name);
            //            string rowString = string.Format("{0},{1}", chromosome, name);
            //            sw.WriteLine(rowString);
            //        }
            //    }
            //}
            #endregion

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Console.WriteLine("需求  devideFirstColumns.tsv 文件的第一列variant (1:15791:C:T)    与 snp150_hg19.txt文件第一列 chromosome:start(1:15791)  匹配，然后将 chromosome:start  name  两列写入 devideFirstColumns.tsv 文件\r\n\r\n");
            try
            {
                #region devideFirstColumns.tsv
                var tsvPath = @"D:/mendia/data/diabetes/2443.gwas.imputed_v3.both_sexes/devideFirstColumns.tsv";

                Console.WriteLine($"开始读取 {tsvPath}：时间：{DateTime.Now}");
                // 读取 tsv 文件内容到字符串数组中  
                string[] lines = File.ReadAllLines(tsvPath);

                stopWatch.Stop();
                Console.WriteLine($"结束读取 {tsvPath}：读取行数：{lines.Count()},时间：{DateTime.Now} 耗时：{stopWatch.Elapsed.TotalMilliseconds}毫秒");
                stopWatch.Start();

                //var contentColumns = lines.ToList()
                //    .Select(line => line.Split('\t').First())
                //    .Where(m => m.Contains(":"))
                //    .Select(x => x.Substring(0, x.LastIndexOf(":")))
                //    .Select(y => y.Substring(0, y.LastIndexOf(":")))
                //    .ToList();

                //stopWatch.Stop();
                //Console.WriteLine($"计算devideFirstColumns.tsv contentColumns 读取第一行数据：{contentColumns.Count},时间：{DateTime.Now} 耗时：{stopWatch.Elapsed.TotalMilliseconds}毫秒");
                //stopWatch.Start();
                #endregion


                #region snp150_hg19.txt

                var snpPath = @"D:/mendia/data/diabetes/2443.gwas.imputed_v3.both_sexes/snp150_hg19.txt";
                //var snpList = new List<SnpInfo> { new SnpInfo { ChromosomeStart = @"chromosome:start", Name = "name" } };
                //var snpList = MiniExcelLibs.MiniExcel.Query<SnpInfo>(snpPath);
                //foreach (var snp in snpList)
                //{
                //    Console.ReadKey();
                //}
                Console.WriteLine($"开始读取{snpPath}数据到内存 时间：{DateTime.Now}");

                // 读取 tsv 文件内容到字符串数组中  
                var snpLines = File.ReadAllLines(snpPath)
                    .Select(snp => new SnpInfo { ChromosomeStart = snp.Split('\t')[0], Name = snp.Split('\t')[1] });
                    //.ToList();
                //snpLines.RemoveAt(0);

                //foreach (var snpLine in snpLines)
                //{
                //    // 处理每一行
                //    Console.WriteLine(snpLine);
                //    var items = snpLine.Split('\t');
                //    if (items.Length == 2)
                //    {
                //        var chromosome = items[0];
                //        var snpName = items[1];

                //        if (contentColumns.Contains(chromosome))
                //        {
                //            Console.WriteLine($"snpList.Add(chromosome:{chromosome},SNP:{snpName})");
                //            snpList.Add(new SnpInfo { ChromosomeStart = chromosome, Name = snpName });
                //        }
                //    }
                //}

                #endregion

                stopWatch.Stop();
                Console.WriteLine($"结束读取{snpPath}数据到内存 成功! 时间：{DateTime.Now}  耗时：{stopWatch.Elapsed.TotalMilliseconds}毫秒");
                stopWatch.Start();


                // 将新的行数据写入文件  
                var newTsvPath = @"D:/mendia/data/diabetes/2443.gwas.imputed_v3.both_sexes/newTsvFile.tsv";

                Console.WriteLine($"开始查找 {tsvPath}与{snpPath}匹配并写入{newTsvPath} 时间：{DateTime.Now}");

                // 读取并处理每一行

                var newColumnData = new List<string>();
                // 创建一个 StreamWriter 对象，使用 UTF-8 编码  
                using (StreamWriter writer = new StreamWriter(newTsvPath))
                {
                    foreach (var line in lines)
                    {
                        newColumnData.Clear();
                        // 使用逗号分隔行内容，并添加新列数据  
                        var columns = line.Split('\t');
                        var firstColumn = columns[0];
                        var firstColumns = firstColumn.Split(":");
                        if (firstColumns.Length == 4)
                        {
                            firstColumn = firstColumn.Substring(0, firstColumn.LastIndexOf(":"));
                            firstColumn = firstColumn.Substring(0, firstColumn.LastIndexOf(":"));
                            var snp = snpLines?.FirstOrDefault(x => x.ChromosomeStart == firstColumn);
                            if(snp is null)
                            {
                                Console.WriteLine($"{firstColumn}匹配失败! 忽略");
                                continue;
                            }
                            newColumnData.Add(snp.ChromosomeStart);
                            newColumnData.Add(snp.Name);
                        }
                        else if (firstColumn.Equals("variant"))
                        {
                            newColumnData.Add("chromosome:start");
                            newColumnData.Add("SNP");
                        }
                        writer.WriteLine(string.Join("\t", newColumnData.Concat(columns).ToArray()));
                    }
                }

                stopWatch.Stop();
                Console.WriteLine($"查找 {tsvPath}与{snpPath} 时间：{DateTime.Now},耗时：{stopWatch.Elapsed.TotalMilliseconds}毫秒");
            }
            catch (Exception ex)
            {
                stopWatch.Stop();
                Console.WriteLine($"执行错误：{ex.Message}");
            }
            finally
            {
                Console.WriteLine($"程序结束,时间：{DateTime.Now} 耗时：{stopWatch.Elapsed.TotalMilliseconds}毫秒");
            }
        }
    }
}
