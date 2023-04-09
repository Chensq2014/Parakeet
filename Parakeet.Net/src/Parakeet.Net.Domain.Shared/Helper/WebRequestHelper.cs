using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Parakeet.Net.Dtos.Requests;

namespace Parakeet.Net.Helper
{
    public class WebRequestHelper
    {

		/// <summary>
		/// 使用插件 首先应在es容器里安装对应版本插件
		/// https://github.com/NLPchina/elasticsearch-sql
		/// 查询时使用/_nlpcn/sql 让插件将sql语句翻译为dcl语法执行
		/// </summary>
		/// <param name="queryParam"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static string Post(string queryParam, string url = "http://localhost:9200/_nlpcn/sql")
		{
			HttpWebRequest request = null;
			try
			{
				request = (HttpWebRequest)WebRequest.Create(url);
				var data = Encoding.UTF8.GetBytes(queryParam);
				request.Accept = "application/json; charset=UTF-8"; // 设置响应数据的ContentType
				request.Method = "POST";
				request.ContentType = "application/json"; // 设置请求数据的ContentType
				request.ContentLength = data.Length;
				request.Timeout = 90000;
				// 设置入参
				using (var stream = request.GetRequestStream())
				{
					stream.Write(data, 0, data.Length);
				}
				// 发送请求
				var response = (HttpWebResponse)request.GetResponse();
				// 读取出参
				using (var resStream = response.GetResponseStream())
				{
					using (var reader = new StreamReader(resStream, Encoding.UTF8))
					{
						return reader.ReadToEnd();
					}
				}
			}
			catch (Exception ex)
			{
				return null;
			}
			finally
			{
				// 释放连接
				if (request != null) request.Abort();
			}
		}

		public static string Post(QueryParamDto queryParam, string url = "http://localhost:9200/_xpack/sql?format=csv")
		{
			HttpWebRequest request = null;
			try
			{
				request = (HttpWebRequest)WebRequest.Create(url);
				var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(queryParam));
				//request.Accept = "application/json; charset=UTF-8"; // 设置响应数据的ContentType
				request.Method = "POST";
				request.ContentType = "application/json"; // 设置请求数据的ContentType
				request.ContentLength = data.Length;
				request.Timeout = 90000;
				// 设置入参
				using (var stream = request.GetRequestStream())
				{
					stream.Write(data, 0, data.Length);
				}
				// 发送请求
				var response = (HttpWebResponse)request.GetResponse();
				// 读取出参
				using (var resStream = response.GetResponseStream())
				{
					using (var reader = new StreamReader(resStream, Encoding.UTF8))
					{
						return reader.ReadToEnd();
					}
				}
			}
			catch (Exception ex)
			{
				return null;
			}
			finally
			{
				// 释放连接
				if (request != null) request.Abort();
			}
		}


		public static DataTable GetDataBySql(string sql, string url = "http://192.168.1.211:9200/_xpack/sql?format=csv")
		{
			DataTable dataTable = new DataTable();
			try
			{
				// 调用通过sql去查询结果
				string jsonstr = Post(new QueryParamDto() { Query = sql });

				var lines = jsonstr.Split("\r\n");
				foreach (string item in lines[0].Split(","))
				{
					//可以做类型转换
					dataTable.Columns.Add(item, typeof(System.String));
				}
				for (int i = 1; i < lines.Length - 1; i++)
				{
					var filedvalue = lines[i].Split(",");
					var row = dataTable.NewRow();
					for (int j = 0; j < dataTable.Columns.Count; j++)
					{
						row[j] = filedvalue[j];
					}
					dataTable.Rows.Add(row);
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			return dataTable;

		}
	}
}
