using JiebaNet.Segmenter;
using JiebaNet.Segmenter.Common;
using Microsoft.Win32;
using MiniExcelLibs;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using WpfSegment.Models;

namespace WpfSegment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string SourceFilePath { get; set; }

        public string TargetFilePath { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SourceFilePath = Path.Text;
            TargetFilePath = System.IO.Path.GetDirectoryName(SourceFilePath) + "\\" + System.IO.Path.GetFileNameWithoutExtension(SourceFilePath) + "_统计结果_" + DateTime.Now.ToString("yyyyMMddHHmmss") + System.IO.Path.GetExtension(SourceFilePath);
        }

        private void SelectSourceFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select a File",
                InitialDirectory = "C:\\",
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FilterIndex = 2,
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                Path.Text = fileName;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConfigManager.ConfigFileBaseDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Jieba\\Resources";
            var jiebaSegmenter = new JiebaSegmenter();
            var sheetNames = MiniExcel.GetSheetNames(SourceFilePath);
            var list = new List<IssueModel>();
            foreach (string sheetName in sheetNames)
            {
                var models = MiniExcel.Query<IssueModel>(SourceFilePath, sheetName, ExcelType.UNKNOWN, "A2").ToList();
                foreach (var item in models)
                {
                    if (string.IsNullOrWhiteSpace(item.ProblemDescription))
                    {
                        continue;
                    }
                    var counter = new Counter<string>(jiebaSegmenter.Cut(item.ProblemDescription));
                    if (counter == null)
                    {
                        continue;
                    }
                    foreach (var item3 in counter.MostCommon())
                    {
                        item.WordCounts.Add(new WordCountDto
                        {
                            GroupByType = "按问题描述",
                            Word = item3.Key,
                            Count = item3.Value
                        });
                    }
                }
                list.AddRange(models);
            }
            if (list.Any())
            {
                var second = list
                    .SelectMany(x => x.WordCounts)
                    .Where(x => x.Word.Trim().Length >= 2)
                    .GroupBy(y => y.Word)
                    .Select(z => new WordCountDto
                    {
                        GroupByType = "按问题描述",
                        Word = z.Key,
                        Count = z.Sum((WordCountDto o) => o.Count.GetValueOrDefault())
                    }).OrderByDescending(w => w.Count);
                var second2 = list
                    .GroupBy(x => x.IssueType)
                    .Select(y => new WordCountDto
                    {
                        GroupByType = "按问题类型",
                        Word = (string.IsNullOrWhiteSpace(y.Key) ? "未填写" : y.Key),
                        Count = y.Count()
                    }).OrderByDescending(z => z.Count);
                var second3 = list
                    .GroupBy(x => x.Department)
                    .Select(y => new WordCountDto
                    {
                        GroupByType = "按末级部门",
                        Word = (string.IsNullOrWhiteSpace(y.Key) ? "未填写" : y.Key),
                        Count = y.Count()
                    }).OrderByDescending(z => z.Count);
                var first = list
                    .GroupBy(x => x.Division)
                    .Select(y => new WordCountDto
                    {
                        GroupByType = "按事业部/中心",
                        Word = (string.IsNullOrWhiteSpace(y.Key) ? "未填写" : y.Key),
                        Count = y.Count()
                    }).OrderByDescending(z => z.Count);

                MiniExcel.SaveAs(TargetFilePath, first.Union(second3).Union(second2).Union(second));
                MessageBox.Show("统计结果已存入：" + TargetFilePath + "请查阅");
                //Process.Start(TargetFilePath);//反编译出来的？
            }
            else
            {
                MessageBox.Show("没有读取到有效数据请检查源文件:" + SourceFilePath);
            }
        }
    }
}