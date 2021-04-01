using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace SPS
{
    public class ScanningHelper
    {
        public static MainWindow mainWindow;
        public static string Target = null;
        public static int Threads = 20;
        private static Queue<string> FileNames = new Queue<string>();
        private static Queue<string> ScaningDatas = new Queue<string>();
        private static int TimeOut = 100000;
        private static string Protocol = "";
        public static bool Message = true;
        /// <summary>
        /// 绑定MainWindow窗体的所有事件
        /// </summary>
        public static void MainWindowEventBinding()
        {
            mainWindow.Threads.TextChanged += Threads_TextChanged;
            mainWindow.Up.Click += UpDownButton_Click;
            mainWindow.Down.Click += UpDownButton_Click;
            mainWindow.Loaded += mainWindow_Loaded;
            mainWindow.StartPause.Click += StartPause_Click;
            mainWindow.Reset.Click += ReSetData;
            mainWindow.listbox200.MouseDoubleClick += OpenBroser;
            mainWindow.listbox3XX.MouseDoubleClick += OpenBroser;
            mainWindow.listbox403.MouseDoubleClick += OpenBroser;
        }
        public static void OpenBroser(object sender, MouseButtonEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            if (listBox.SelectedItem != null)
            {
                Process.Start("explorer.exe", listBox.SelectedItem.ToString());
            }
        }
        /// <summary>
        /// 扫描前的准备工作
        /// </summary>
        private static void StartPause_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                mainWindow.Dispatcher.Invoke(new Action(() =>
                {
                    mainWindow.Reset.IsEnabled = true;
                    Threads = Convert.ToInt32(mainWindow.Threads.Text);
                    ThreadPool.SetMinThreads(Threads, Threads);
                    Target = mainWindow.Target.Text;
                    Button button = (Button)sender;
                    if (button.Content.ToString() == "启动")
                    {
                        try
                        {
                            TimeOut = Convert.ToInt32(mainWindow.TimeOut.Text.Replace(" ", "")) * 1000;
                            Protocol = mainWindow.Protocol.Text.Replace(" ", "");
                            if (Target.Replace("http://", "").Replace("https://", "").Trim() == String.Empty)
                            {
                                MessageBox.Show("目标站点不能为空");
                            }
                            else
                            {
                                //添加字典文件列表
                                foreach (CheckBox checkBox in mainWindow.FileGrid.Children)
                                {
                                    if (checkBox.IsChecked.Value)
                                    {
                                        if (!FileNames.Contains(checkBox.Content.ToString()))
                                        {
                                            FileNames.Enqueue(checkBox.Content.ToString());
                                        }
                                    }
                                }
                                //添加字典文件列表
                                //读取所有字典
                                if (FileNames.Count >= 1)
                                {
                                    Task LoadFiles = Task.Run(() =>
                                    {
                                        while (FileNames.Count >= 1)
                                        {
                                            byte[] buff;
                                            FileStream fileStream = new FileStream(FileNames.Dequeue(), FileMode.Open);
                                            buff = new byte[fileStream.Length];
                                            fileStream.Read(buff, 0, buff.Length);
                                            fileStream.Flush();
                                            fileStream.Close();
                                            string TmpStringDatas = Encoding.Default.GetString(buff);
                                            TmpStringDatas = TmpStringDatas.Replace("\r", String.Empty);
                                            buff = null;
                                            string[] Datas = TmpStringDatas.Split('\n').ToArray();
                                            foreach (string Data in Datas)
                                            {
                                                if (!ScaningDatas.Contains(Data) && Data.Replace(" ", "") != String.Empty && Data != null)
                                                {
                                                    if (Data[0] == '/')
                                                    {
                                                        ScaningDatas.Enqueue(Data.Replace(" ", ""));
                                                    }
                                                    else
                                                    {
                                                        ScaningDatas.Enqueue("/" + Data.Replace(" ", ""));
                                                    }
                                                }
                                            }
                                        }
                                    });
                                    LoadFiles.ContinueWith(T =>
                                    {
                                        Message = true;
                                        mainWindow.Dispatcher.Invoke(new Action(() =>
                                        {
                                            mainWindow.ProgressBar1.Opacity = 1.0;
                                            mainWindow.ProgressBar1.Maximum = ScaningDatas.Count;
                                            mainWindow.StartPause.Content = "暂停";
                                        }));
                                        if (!HttpState) HttpState = true;
                                        for (int i = 0; i < Threads; i++)
                                        {
                                            Scaning();//循环调用线程扫描
                                        }
                                    }, TaskContinuationOptions.AttachedToParent);
                                }
                                else
                                {
                                    MessageBox.Show("您的字典为空");
                                }
                                //读取所有字典
                            }
                        }
                        catch
                        {
                            mainWindow.TimeOut.Text = "5";
                            MessageBox.Show("超时参数有误");
                        }
                    }
                    else if (button.Content.ToString() == "暂停")
                    {
                        HttpState = false;
                        mainWindow.StartPause.Content = "继续";
                    }
                    else if (button.Content.ToString() == "继续")
                    {
                        mainWindow.Reset.IsEnabled = false;
                        HttpState = true;
                        mainWindow.StartPause.Content = "暂停";
                    }
                }));
            });
        }
        private static bool HttpState = true;//一个暂停的锁
        private static bool Reset = false;
        private static void Scaning()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (ScaningDatas.Count >= 1)
                    {
                        if (Reset)
                        {
                            Reset = false;
                            break;
                        }
                        if (HttpState)
                        {
                            string T = ScaningDatas.Dequeue();
                            T = Target + T;
                            T = T.Replace("//", "/");
                            T = T.Insert(6, "/");
                            if (T != null)
                            {
                                HttpWebRequest httpWebRequest = null;
                                HttpWebResponse httpWebResponse = null;
                                try
                                {
                                    httpWebRequest = WebRequest.Create(T) as HttpWebRequest;
                                    httpWebRequest.Method = Protocol;
                                    httpWebRequest.Timeout = TimeOut;
                                    httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                    int StatusCode = Convert.ToInt32(httpWebResponse.StatusCode);
                                    string RequestUri = httpWebRequest.RequestUri.ToString();
                                    if (RequestUri[RequestUri.Length - 1] == '/')
                                    {
                                        RequestUri = RequestUri.Remove(RequestUri.Length - 1);
                                    }
                                    mainWindow.Dispatcher.Invoke(new Action(() =>
                                    {
                                        if (mainWindow.Checkbox_200.IsChecked.Value)
                                        {
                                            if (StatusCode == 200 && httpWebResponse.ResponseUri.AbsoluteUri == httpWebRequest.RequestUri.AbsoluteUri)
                                            {
                                                if (!mainWindow.listbox200.Items.Contains(RequestUri))
                                                {
                                                    mainWindow.listbox200.Items.Add(RequestUri);
                                                }
                                            }
                                        }
                                        if (mainWindow.Checkbox_3XX.IsChecked.Value)
                                        {
                                            if (StatusCode.ToString()[0] == '3')
                                            {
                                                if (!mainWindow.listbox3XX.Items.Contains(RequestUri))
                                                {
                                                    mainWindow.listbox3XX.Items.Add(RequestUri);
                                                }
                                            }
                                        }
                                        httpWebResponse.Close();
                                    }));
                                }
                                catch (WebException Ex)
                                {
                                    if (Ex.Status == WebExceptionStatus.ProtocolError)
                                    {
                                        string ResponseUri = Ex.Response.ResponseUri.ToString();
                                        if (ResponseUri[ResponseUri.Length - 1] == '/')
                                        {
                                            ResponseUri = ResponseUri.Remove(ResponseUri.Length - 1);
                                        }
                                        int StatusCode = Convert.ToInt32(Regex.Match(Ex.Message, "([0-9]{3})").Groups[1].Value);
                                        mainWindow.Dispatcher.Invoke(new Action(() =>
                                        {
                                            if (mainWindow.Checkbox_403.IsChecked.Value)
                                            {
                                                if (StatusCode == 403)
                                                {
                                                    if (!mainWindow.listbox403.Items.Contains(ResponseUri))
                                                    {
                                                        mainWindow.listbox403.Items.Add(ResponseUri);
                                                    }
                                                }
                                            }
                                        }));
                                    }
                                }
                                finally
                                {
                                    mainWindow.Dispatcher.Invoke(new Action(() =>
                                    {
                                        mainWindow.ProgressBar1.Value++;
                                    }));
                                }
                            }
                        }
                        else
                        {
                            Task.Delay(10);
                        }
                    }
                    else
                    {
                        if (Message)
                        {
                            Message = false;
                            mainWindow.Dispatcher.Invoke(() =>
                            {
                                mainWindow.StartPause.Content = "启动";
                            });
                            MessageBox.Show("扫描任务执行完毕");
                        }
                        break;
                    }
                }
            });
        }
        /// <summary>
        /// 动态创建字典列表控件
        /// </summary>
        private static void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                mainWindow.Dispatcher.Invoke(new Action(() =>
                {
                    List<string> DirInfo = new List<string>();
                    DirectoryInfo directory = new DirectoryInfo("./");
                    FileInfo[] Dir = directory.GetFiles();
                    int CheckBoxIndex = 1;
                    foreach (var filename in Dir)
                    {
                        if (filename.ToString().ToLower().Contains(".txt"))
                        {
                            if (!FileNames.Contains(filename.ToString()))
                            {
                                CheckBox checkBox = new CheckBox();
                                checkBox.Name = "CheckBox" + CheckBoxIndex.ToString();
                                checkBox.Content = filename.ToString();
                                mainWindow.FileGrid.Children.Add(checkBox);
                                CheckBoxIndex++;
                                DirInfo.Add(filename.ToString());
                                checkBox.IsChecked = true;
                            }
                        }
                    }
                    mainWindow.ProgressBar1.Opacity = 0.0;
                    mainWindow.Protocol.SelectedIndex = 0;
                }));
            });
        }
        private static void Threads_TextChanged(object sender, TextChangedEventArgs e)
        {
            Task.Run(() =>
            {
                ((TextBox)sender).Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        ((TextBox)sender).Text = Convert.ToInt32(((TextBox)sender).Text.Trim()).ToString();
                        if (Convert.ToInt32(((TextBox)sender).Text.Trim()) == 0 || Convert.ToInt32(((TextBox)sender).Text.Trim()) > 50)
                        {
                            ((TextBox)sender).Text = "20";
                            MessageBox.Show("线程数在20条左右更合适哦");
                        }
                    }
                    catch
                    {
                        ((TextBox)sender).Text = "20";
                    }
                }));
            });
        }
        private static void UpDownButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                mainWindow.Threads.Dispatcher.Invoke(new Action(() =>
                {
                    if (((Button)sender).Name == "Up" && Convert.ToInt32(mainWindow.Threads.Text.Trim()) != 50)
                    {
                        mainWindow.Threads.Text = (Convert.ToInt32(mainWindow.Threads.Text.Trim()) + 1).ToString();
                    }
                    else if (((Button)sender).Name == "Down" && Convert.ToInt32(mainWindow.Threads.Text.Trim()) != 1)
                    {
                        mainWindow.Threads.Text = (Convert.ToInt32(mainWindow.Threads.Text.Trim()) - 1).ToString();
                    }
                }));
            });
        }
        /// <summary>
        /// 重置方法
        /// </summary>
        private static void ReSetData(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                HttpState = false;
                Reset = true;
                mainWindow.Dispatcher.Invoke(new Action(() =>
                {
                    mainWindow.Reset.IsEnabled = false;
                    mainWindow.StartPause.Content = "启动";
                    mainWindow.listbox200.Items.Clear();
                    mainWindow.listbox403.Items.Clear();
                    mainWindow.listbox3XX.Items.Clear();
                    mainWindow.ProgressBar1.Value = 0;
                    mainWindow.ProgressBar1.Opacity = 0.0;
                }));
                FileNames.Clear();
                ScaningDatas.Clear();
                GC.Collect();
                HttpState = true;
                Reset = false;
            });
        }
    }
}
