using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Parakeet.Net.Extensions
{
    /// <summary>
    /// 提供Excel导入导出及文件等扩展
    /// 静态类中的静态方法不占内存，普通变量也会释放
    /// 只有静态类中的静态字段常驻内存
    /// </summary>
    public class FileExtension
    {
        #region 内存流写入文件

        /// <summary>
        /// 内存流写入文件
        /// </summary>
        /// <param name="ms">内存流</param>
        /// <param name="filePath">创建文件的物理地址</param>
        public static void MemoryToFile(MemoryStream ms, string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            if (ms.CanWrite)
            {
                using (fs)
                {
                    ms.WriteTo(fs);
                    fs.Flush();
                    fs.Close();
                }
                ms.Close();
            }
        }

        #endregion

        #region 保存文件
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="filePath">文件物理地址</param>
        public static async Task SaveFile(IFormFile file, string filePath)
        {
            //File.Copy(file.FileName, filePath);//CreateNew
            await using var fs = new FileStream(filePath, FileMode.CreateNew);
            await file.CopyToAsync(fs); //CopyTo(fs);//
            //fs.Flush();
        }

        /// <summary>
        /// 保存覆盖原文件
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="filePath">文件物理地址</param>
        public static async Task SaveOrUpdateFile(IFormFile file, string filePath)
        {
            //File.Copy(file.FileName, filePath);//CreateNew
            await using var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            await file.CopyToAsync(fs); //CopyTo(fs);//
            //fs.Flush();
        }

        #endregion

        #region 保存文件目录/文件
        // 检查是否要创建上传文件夹
        public static bool CreateFolderIfNeeded(string path)
        {
            bool success = true;
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    //TODO：处理异常
                    success = false;
                }
            }
            return success;
        }

        /// <summary>
        /// 清空指定目录下的文件
        /// </summary>
        /// <param name="targetPath"></param>
        public static void ClearDir(string targetPath)
        {
            if (Directory.Exists(targetPath))
            {
                //var dirInfo = new DirectoryInfo(targetPath);
                //foreach (var file in dirInfo.GetFiles())
                //{
                //    file.Delete();
                //}
                //Directory.Delete(targetPath);
                Directory.Delete(targetPath, true);//删除这个目录及文件
            }
        }

        /// <summary>
        /// 删除指定目录下的文件后，检查目录是否为空，为空就删除目录
        /// </summary>
        /// <param name="filePath"></param>
        public static void CheckClearParentDir(string filePath)
        {
            var dirInfo = new DirectoryInfo(filePath);
            if (dirInfo.Parent != null)
            {
                if (dirInfo.Parent.Exists)
                {
                    if (dirInfo.Parent.GetFiles().Any())
                    {
                        return;//还有文件就不删除目录
                    }
                    Directory.Delete(dirInfo.Parent.FullName);
                }
            }
        }

        /// <summary>
        /// 根据文件全名删除文件
        /// </summary>
        /// <param name="fileFullName"></param>
        public static void ClearFile(string fileFullName)
        {
            if (File.Exists(fileFullName))
            {
                File.Delete(fileFullName);
            }
        }

        /// <summary>
        /// 查找指定路劲下所有目录
        /// </summary>
        /// <param name="rootPath"></param>
        /// <returns></returns>
        public static List<DirectoryInfo> GetAllDirectories(string rootPath)
        {
            var directories = new List<DirectoryInfo>();
            if (!Directory.Exists(rootPath))
            {
                return directories;
            }
            var directory = new DirectoryInfo(rootPath);
            directories.Add(directory);
            foreach (var directoryInfo in directory.GetDirectories())
            {
                directories.AddRange(GetAllDirectories(directoryInfo.FullName));
            }
            return directories;
        }

        #endregion

        #region 根据XSSFWorkbook创建Excel文件
        /// <summary>
        /// 根据XSSFWorkbook创建Excel文件
        /// </summary>
        /// <param name="workbook">XSSFWorkbook工作簿</param>
        /// <param name="filePath">创建文件的物理地址</param>
        public static void WriteWorkbookToFile(IWorkbook workbook, string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            using (fs)
            {
                workbook.Write(fs);
                fs.Close();
            }
        }

        #endregion

        #region 将文件转换为byte[]

        /// <summary>   
        /// 将文件转换为byte[]
        /// </summary>   
        /// <param name="path">图片路径</param>   
        /// <returns>二进制数组</returns>   
        public static async Task<byte[]> GetBinaryData(string path)
        {
            await using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            var imageBytes = await GetBinaryData(fs);
            await fs.FlushAsync();
            fs.Close();
            return imageBytes;
        }

        /// <summary>   
        /// 内存流转换为byte[]
        /// </summary>   
        /// <param name="fs">文件流/内存流</param>
        /// <returns>二进制数组</returns>
        public static async Task<byte[]> GetBinaryData(Stream fs)
        {
            var imageBytes = new byte[fs.Length];
            await fs.ReadAsync(imageBytes, 0, Convert.ToInt32(fs.Length));
            return imageBytes;
        }
        #endregion

        #region File Compress 

        /// <summary>
        /// todo:压缩文件之后打不开bug？
        /// </summary>
        /// <param name="orgPath"></param>
        /// <param name="compressPath"></param>
        public static void CompressFile(string orgPath, string compressPath)
        {
            using FileStream originalFileStream = File.Open(orgPath, FileMode.Open);
            using FileStream compressedFileStream = File.Create(compressPath);
            using var compressor = new DeflateStream(compressedFileStream, CompressionMode.Compress);
            originalFileStream.CopyTo(compressor);
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="compressPath"></param>
        /// <param name="newPath"></param>
        public static void DecompressFile(string compressPath, string newPath)
        {
            using FileStream compressedFileStream = File.Open(compressPath, FileMode.Open);
            using FileStream outputFileStream = File.Create(newPath);
            using var decompressor = new DeflateStream(compressedFileStream, CompressionMode.Decompress);
            decompressor.CopyTo(outputFileStream);
        }

        /// <summary>
        /// Zip压缩文件
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="zipFile"></param>
        /// <exception cref="Exception"></exception>
        public static void ZipCompressFile(string sourceFile, string zipFile)
        {
            try
            {
                using var zip = ICSharpCode.SharpZipLib.Zip.ZipFile.Create(zipFile);
                zip.BeginUpdate();
                zip.Add(sourceFile, Path.GetFileName(sourceFile));
                zip.CommitUpdate();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to zip file {sourceFile}", ex);
            }
        }

        /// <summary>
        /// 压缩目录到文件
        /// </summary>
        /// <param name="fromDir"></param>
        /// <param name="newFile"></param>
        public static void CreateZipFromDirectory(string fromDir, string newFile)
        {
            if (File.Exists(newFile))
            {
                File.Delete(newFile);
            }
            ZipFile.CreateFromDirectory(fromDir, newFile);
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void UnZip(string source, string target)
        {
            try
            {
                ZipFile.ExtractToDirectory(source, target);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to zip {source}:{ex.Message}", ex);
            }
        }

        //private static void PrintResults()
        //{
        //    long originalSize = new FileInfo(OriginalFileName).Length;
        //    long compressedSize = new FileInfo(CompressedFileName).Length;
        //    long decompressedSize = new FileInfo(DecompressedFileName).Length;

        //    Console.WriteLine($"The original file '{OriginalFileName}' weighs {originalSize} bytes. Contents: \"{File.ReadAllText(OriginalFileName)}\"");
        //    Console.WriteLine($"The compressed file '{CompressedFileName}' weighs {compressedSize} bytes.");
        //    Console.WriteLine($"The decompressed file '{DecompressedFileName}' weighs {decompressedSize} bytes. Contents: \"{File.ReadAllText(DecompressedFileName)}\"");
        //}

        //private static void DeleteFiles()
        //{
        //    File.Delete(OriginalFileName);
        //    File.Delete(CompressedFileName);
        //    File.Delete(DecompressedFileName);
        //}



        #endregion

    }
}
