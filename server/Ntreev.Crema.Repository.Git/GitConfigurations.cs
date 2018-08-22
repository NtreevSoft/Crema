using Ntreev.Crema.Services;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Git
{
    [Export(typeof(IConfigurationPropertyProvider))]
    class GitConfigurations : IConfigurationPropertyProvider, INotifyPropertyChanged
    {
        public GitConfigurations()
        {

        }

        [ConfigurationProperty(ScopeType = typeof(ICremaConfiguration))]
        [DefaultValue(null)]
        [Description("git의 실행 경로를 나타냅니다.")]
        public string ExecutablePath
        {
            get => GitCommand.ExecutablePath;
            set
            {
                GitCommand.ExecutablePath = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(ExecutablePath)));
            }
        }

        [ConfigurationProperty(ScopeType = typeof(ICremaConfiguration))]
        [DefaultValue(0)]
        [Description("표시될 로그의 최대 개수를 나타냅니다.")]
        public int MaxLogCount
        {
            get => GitLogInfo.MaxCount;
            set
            {
                GitLogInfo.MaxCount = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(MaxLogCount)));
            }
        }

        public string Name => "git";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }
    }
}
