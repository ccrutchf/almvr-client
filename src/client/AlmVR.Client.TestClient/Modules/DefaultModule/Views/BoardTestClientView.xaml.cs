using AlmVR.Client.TestClient.Modules.DefaultModule.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AlmVR.Client.TestClient.Modules.DefaultModule.Views
{
    /// <summary>
    /// Interaction logic for BoardTestClientView.xaml
    /// </summary>
    public partial class BoardTestClientView : TabControlViewBase
    {
        public BoardTestClientView(BoardTestClientViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
