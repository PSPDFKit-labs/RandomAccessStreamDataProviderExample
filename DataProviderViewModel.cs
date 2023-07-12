using PSPDFKit.Document;
using PSPDFKit.UI;
using PSPDFKitFoundation.Data;
using System;
using System.Windows.Input;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.Popups;

namespace RandomAccessStreamDataProviderExample
{
    /// <summary>
    /// This example demonstrates how to open and export a PDF using a DataProvider and DataSink.
    /// It uses <see cref="RandomAccessStreamDataProvider"/> and
    /// <see cref="RandomAccessStreamDataSink"/> which simply wrap a <see cref="IRandomAccessStream"/>.
    /// </summary>
    public class DataProviderViewModel : Observable
    {
        private ICommand _openPDFCommand;
        private ICommand _savePDFCommand;

        public ICommand OpenPDFCommand => _openPDFCommand ?? (_openPDFCommand = new RelayCommand(OpenPDFAsync));
        public ICommand SavePDFCommand => _savePDFCommand ?? (_savePDFCommand = new RelayCommand(SavePDFAsync));

        public bool FlattenAnnotations { get; set; } = false;
        public bool IncrementalSave { get; set; } = true;

        public PdfView PDFView { get; private set; }

        private IRandomAccessStream _fileStream;

        /// <summary>
        /// Save to the same file that was opened.
        /// If annotations are flattened they will be flattened in the open document too.
        /// </summary>
        public async void SavePDFAsync()
        {
            try
            {
                DocumentExportOptions exportOptions = new DocumentExportOptions
                {
                    Flattened = FlattenAnnotations,
                    Incremental = IncrementalSave, // Note that if `Flattened` is true then this property has no effect.
                };

                DataSinkOption option = DataSinkOption.Append;
                if (FlattenAnnotations || !IncrementalSave)
                {
                    // Both flattening and incremental saves cause the entire document to be written out
                    // So we need to reposition the stream to the beginning and truncate it.
                    _fileStream.Seek(0);
                    _fileStream.Size = 0;
                    option = DataSinkOption.NewFile;
                }

                await PDFView.Document.ExportToDataSinkAsync(new ExampleDataSink(option) { Stream = _fileStream }, exportOptions);

            }
            catch (Exception ex)
            {
                MessageDialog messageDialog = new MessageDialog(ex.Message);
                await messageDialog.ShowAsync();
            }
        }

        public async void OpenPDFAsync()
        {
            try
            {
                StorageFile file = await FileUtils.PickFileToOpenAsync(".pdf");
                if (file == null)
                {
                    return;
                }

                _fileStream?.Dispose();
                _fileStream = await file.OpenAsync(FileAccessMode.ReadWrite);
                ExampleDataProvider dataProvider = new ExampleDataProvider(_fileStream);
                DocumentSource documentSource = DocumentSource.CreateFromDataProvider(dataProvider);
                await PDFView.Controller.ShowDocumentAsync(documentSource);
            }
            catch (Exception ex)
            {
                MessageDialog messageDialog = new MessageDialog(ex.Message);
                await messageDialog.ShowAsync();
            }
        }

        public void Initialize(PdfView pdfView)
        {
            PDFView = pdfView;
            PDFView.ShowMessage("Please press the 'Open' button to open a PDF.");
        }

        public void ViewUnloaded()
        {
            if (_fileStream == null)
            {
                return;
            }

            _fileStream.Dispose();
            _fileStream = null;
        }
    }
}
