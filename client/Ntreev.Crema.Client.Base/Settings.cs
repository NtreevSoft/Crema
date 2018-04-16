//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Ntreev.Crema.Client.Base.Properties {
    
    
    // 이 클래스를 사용하여 설정 클래스에 대한 특정 이벤트를 처리할 수 있습니다.
    //  SettingChanging 이벤트는 설정 값이 변경되기 전에 발생합니다.
    //  PropertyChanged 이벤트는 설정 값이 변경된 후에 발생합니다.
    //  SettingsLoaded 이벤트는 설정 값이 로드된 후에 발생합니다.
    //  SettingsSaving 이벤트는 설정 값이 저장되기 전에 발생합니다.
    internal sealed partial class Settings {
        
        public Settings() {
            // // 설정을 저장 및 변경하기 위한 이벤트 처리기를 추가하려면 아래 줄에서 주석 처리를 제거하십시오.
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // SettingChangingEvent 이벤트를 처리하는 코드를 여기에 추가하십시오.
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // SettingsSaving 이벤트를 처리하는 코드를 여기에 추가하십시오.
        }
    }
}
