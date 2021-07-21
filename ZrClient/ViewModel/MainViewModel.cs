using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using ZrClient.API;
using ZrClient.Model;

namespace ZrClient.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        MenuApi api = new MenuApi();
        
        public MainViewModel()
        {
            ModuleGroups = new ObservableCollection<ModuleGroup>();
            TabModels = new ObservableCollection<TabModel>();
            Modules = new ObservableCollection<Modules>();
            Modules modulesModel = new Modules{
                Code = "\ue600",
                Name = "�û�����",
                TypeName = "system.user.User"
            };
            Modules.Add(modulesModel);
            tabIndex = 0;
            changeContentCmd = new RelayCommand<object>(NavChanged);
            ExpandMenuCmd = new RelayCommand(()=> {
                for (int i = 0; i < ModuleGroups.Count; i++)
                {
                    var item = ModuleGroups[i];
                    item.ContractionTemplate = !item.ContractionTemplate;
                }
                Messenger.Default.Send("", "ExpandMenu");
            });
            getMenu();
            NavChanged("home");
        }
        #region =====data
        private ObservableCollection<ModuleGroup> moduleGroups;
        private ObservableCollection<TabModel> tabModels;
        private ObservableCollection<Modules> modules;
        private int tabIndex;
        public int TabIndex
        {
            get { return tabIndex; }
            set { tabIndex = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// ģ�����
        /// </summary>
        private FrameworkElement mainContent;
        public FrameworkElement MainContent
        {
            get { return mainContent; }
            set { mainContent = value; RaisePropertyChanged(); }
        }
        public ObservableCollection<UserModel> GridModelList { get; set; }
        /// <summary>
        /// �Ѽ���ģ��-����
        /// </summary>
        public ObservableCollection<ModuleGroup> ModuleGroups
        {
            get { return moduleGroups; }
            set { moduleGroups = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// �Ѽ���ģ��
        /// </summary>
        public ObservableCollection<Modules> Modules
        {
            get { return modules; }
            set { modules = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// �ѵ��ģ��
        /// </summary>
        public ObservableCollection<TabModel> TabModels
        {
            get { return tabModels; }
            set { tabModels = value; RaisePropertyChanged(); }
        }
        #endregion
        #region ====cmd
        public RelayCommand<object> changeContentCmd { get; set; }
        public RelayCommand ExpandMenuCmd { get; set; }
        #endregion
        private void NavChanged(object o)
        {
            string typeName;
            string tabName;
            if (o.ToString()=="home")
            {
                typeName = "home";
                tabName = "��ҳ";
            }
            else
            {
                var values = (object[])o;
                typeName = values[0].ToString();
                tabName = values[1].ToString();
            }
           
            Type type = Type.GetType("ZrClient.View." + typeName);
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            bool needAdd = true;
            for (int i = 0; i < TabModels.Count; i++)
            {
                if (TabModels[i].Code==o.ToString())
                {
                    TabIndex = i;
                    needAdd = false;
                    break;
                }
            }
            if (needAdd)
            {
                TabModel tabs = new TabModel();
                tabs.Header = tabName;
                tabs.Code = o.ToString();
                tabs.Content = (FrameworkElement)constructor.Invoke(null);
                TabModels.Add(tabs);
                TabIndex = TabModels.Count - 1;
            }
            this.MainContent = (FrameworkElement)constructor.Invoke(null);
        }
        private void getMenu()
        {
            MenuApi mApi = new MenuApi();
            Task.Run(new Action(async()=> {
                var menu = await mApi.getGroup();
                ModuleGroups.Clear();
                foreach (var item in menu)
                {                  
                    ModuleGroups.Add(item);
                }
            }));
        }
    }
}