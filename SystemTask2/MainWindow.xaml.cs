using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace SystemTask2;

public partial class MainWindow : Window
{
    private readonly OpenFileDialog _openFileDialog;
    private readonly CommonOpenFileDialog _openFolderDialog;


    public MainWindow()
    {
        InitializeComponent();
        _openFileDialog = new();
        _openFolderDialog = new();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _openFileDialog.Filter = "Txt files (*.txt)|*.txt";
        _openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        _openFolderDialog.IsFolderPicker = true;
        _openFolderDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    }

    private void btn_Copy_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(tbox_From.Text))
        {
            MessageBox.Show("File path cannot be empty");
            return;
        }

        if (string.IsNullOrEmpty(tbox_To.Text))
        {
            MessageBox.Show("Folder path cannot be empty");
        }

        var bytes = File.ReadAllBytes(tbox_From.Text);

        progressBar.Maximum = bytes.Length;

        var thread = new Thread(() =>
        {
            WriteToFile(bytes);
        });

        thread.IsBackground = true;
        thread.Start();
    }

    private void WriteToFile(byte[] bytes)
    {
        var filePath = string.Empty;

        Dispatcher.Invoke(() =>
        {
            filePath = Path.Combine(tbox_To.Text, _openFileDialog.SafeFileName);
        });

        using var fsSource = new FileStream(filePath, FileMode.Create, FileAccess.Write);

        var dividedBytes = bytes.Chunk(100);

        foreach (var byteArray in dividedBytes)
        {
            fsSource.Write(byteArray, 0, byteArray.Length);

            Dispatcher.Invoke(() =>
            {
                progressBar.Value += byteArray.Length;
            });

            Thread.Sleep(100);
        }

        MessageBox.Show("Completed successfully");
    }

    private void btn_From_Click(object sender, RoutedEventArgs e)
    {
        if (_openFileDialog.ShowDialog() == true)
        {
            tbox_From.Text = _openFileDialog.FileName;
        }
    }

    private void btn_To_Click(object sender, RoutedEventArgs e)
    {
        if (_openFolderDialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            tbox_To.Text = _openFolderDialog.FileName;
        }
    }
}
