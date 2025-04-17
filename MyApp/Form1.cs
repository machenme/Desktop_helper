using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string currentDir = AppDomain.CurrentDomain.BaseDirectory;
                string publicDesktop = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);

                // 自动创建目标目录（如果不存在）
                Directory.CreateDirectory(currentDir);

                int movedFiles = 0;

                foreach (string file in Directory.GetFiles(publicDesktop, "*.*"))
                {
                    try
                    {
                        string rawFileName = Path.GetFileName(file);
                        string sanitizedName = SanitizeFileName(rawFileName);

                        if (string.IsNullOrEmpty(sanitizedName))
                        {
                            MessageBox.Show("跳过空文件名", "警告");
                            continue;
                        }

                        string destFile = Path.Combine(currentDir, sanitizedName);

                        if (File.Exists(destFile))
                        {
                            MessageBox.Show($"文件已存在: {destFile}", "跳过");
                            continue;
                        }

                        File.Move(file, destFile);
                        movedFiles++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"文件 {file} 移动失败: {ex.Message}", "错误");
                    }
                }

                MessageBox.Show($"操作完成！移动了 {movedFiles} 个文件", "信息");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"致命错误: {ex.Message}", "程序错误");
            }
        }

        private string SanitizeFileName(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            return new string(fileName
                .Select(c => invalidChars.Contains(c) ? '_' : c)
                .ToArray());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // 结束explorer进程
                foreach (var process in Process.GetProcessesByName("explorer"))
                {
                    process.Kill();
                }

                // 删除图标缓存
                string cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IconCache.db");

                if (File.Exists(cachePath))
                {
                    File.SetAttributes(cachePath, FileAttributes.Normal);
                    File.Delete(cachePath);
                }

                // 重启explorer
                Process.Start("explorer.exe");

                MessageBox.Show("图标缓存已清理", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
