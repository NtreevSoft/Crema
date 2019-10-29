﻿//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.42000
//
//     파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
//     이러한 변경 내용이 손실됩니다.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ntreev.Crema.Services.UserService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.ntreev.com", ConfigurationName="UserService.IUserService", CallbackContract=typeof(Ntreev.Crema.Services.UserService.IUserServiceCallback), SessionMode=System.ServiceModel.SessionMode.Required)]
    internal interface IUserService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ntreev.com/IUserService/Subscribe", ReplyAction="http://www.ntreev.com/IUserService/SubscribeResponse")]
        Ntreev.Crema.ServiceModel.ResultBase<Ntreev.Crema.ServiceModel.UserContextMetaData> Subscribe(string userID, byte[] password, string version, string platformID, string culture);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ntreev.com/IUserService/Unsubscribe", ReplyAction="http://www.ntreev.com/IUserService/UnsubscribeResponse")]
        Ntreev.Crema.ServiceModel.ResultBase Unsubscribe();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ntreev.com/IUserService/Shutdown", ReplyAction="http://www.ntreev.com/IUserService/ShutdownResponse")]
        Ntreev.Crema.ServiceModel.ResultBase Shutdown(int milliseconds, Ntreev.Crema.ServiceModel.ShutdownType shutdownType, string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ntreev.com/IUserService/CancelShutdown", ReplyAction="http://www.ntreev.com/IUserService/CancelShutdownResponse")]
        Ntreev.Crema.ServiceModel.ResultBase CancelShutdown();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ntreev.com/IUserService/NewUser", ReplyAction="http://www.ntreev.com/IUserService/NewUserResponse")]
        Ntreev.Crema.ServiceModel.ResultBase<Ntreev.Crema.ServiceModel.UserInfo> NewUser(string userID, string categoryPath, byte[] password, string userName, Ntreev.Crema.ServiceModel.Authority authority);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ntreev.com/IUserService/NewUserCategory", ReplyAction="http://www.ntreev.com/IUserService/NewUserCategoryResponse")]
        Ntreev.Crema.ServiceModel.ResultBase NewUserCategory(string categoryPath);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ntreev.com/IUserService/RenameUserItem", ReplyAction="http://www.ntreev.com/IUserService/RenameUserItemResponse")]
        Ntreev.Crema.ServiceModel.ResultBase RenameUserItem(string itemPath, string newName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ntreev.com/IUserService/MoveUserItem", ReplyAction="http://www.ntreev.com/IUserService/MoveUserItemResponse")]
        Ntreev.Crema.ServiceModel.ResultBase MoveUserItem(string itemPath, string parentPath);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ntreev.com/IUserService/DeleteUserItem", ReplyAction="http://www.ntreev.com/IUserService/DeleteUserItemResponse")]
        Ntreev.Crema.ServiceModel.ResultBase DeleteUserItem(string itemPath);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ntreev.com/IUserService/ChangeUserInfo", ReplyAction="http://www.ntreev.com/IUserService/ChangeUserInfoResponse")]
        Ntreev.Crema.ServiceModel.ResultBase<Ntreev.Crema.ServiceModel.UserInfo> ChangeUserInfo(string userID, byte[] password, byte[] newPassword, string userName, System.Nullable<Ntreev.Crema.ServiceModel.Authority> authority);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ntreev.com/IUserService/Kick", ReplyAction="http://www.ntreev.com/IUserService/KickResponse")]
        Ntreev.Crema.ServiceModel.ResultBase Kick(string userID, string comment);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ntreev.com/IUserService/Ban", ReplyAction="http://www.ntreev.com/IUserService/BanResponse")]
        Ntreev.Crema.ServiceModel.ResultBase<Ntreev.Crema.ServiceModel.BanInfo> Ban(string userID, string comment);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ntreev.com/IUserService/Unban", ReplyAction="http://www.ntreev.com/IUserService/UnbanResponse")]
        Ntreev.Crema.ServiceModel.ResultBase Unban(string userID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ntreev.com/IUserService/SendMessage", ReplyAction="http://www.ntreev.com/IUserService/SendMessageResponse")]
        Ntreev.Crema.ServiceModel.ResultBase SendMessage(string userID, string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ntreev.com/IUserService/NotifyMessage", ReplyAction="http://www.ntreev.com/IUserService/NotifyMessageResponse")]
        Ntreev.Crema.ServiceModel.ResultBase NotifyMessage(string[] userIDs, string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ntreev.com/IUserService/IsAlive", ReplyAction="http://www.ntreev.com/IUserService/IsAliveResponse")]
        bool IsAlive();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    internal interface IUserServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://www.ntreev.com/IUserService/OnServiceClosed")]
        void OnServiceClosed(Ntreev.Library.SignatureDate signatureDate, Ntreev.Crema.ServiceModel.CloseInfo closeInfo);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://www.ntreev.com/IUserService/OnUsersChanged")]
        void OnUsersChanged(Ntreev.Library.SignatureDate signatureDate, Ntreev.Crema.ServiceModel.UserInfo[] userInfos);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://www.ntreev.com/IUserService/OnUsersStateChanged")]
        void OnUsersStateChanged(Ntreev.Library.SignatureDate signatureDate, string[] userIDs, Ntreev.Crema.ServiceModel.UserState[] states);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://www.ntreev.com/IUserService/OnUserItemsCreated")]
        void OnUserItemsCreated(Ntreev.Library.SignatureDate signatureDate, string[] itemPaths, System.Nullable<Ntreev.Crema.ServiceModel.UserInfo>[] args);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://www.ntreev.com/IUserService/OnUserItemsRenamed")]
        void OnUserItemsRenamed(Ntreev.Library.SignatureDate signatureDate, string[] itemPaths, string[] newNames);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://www.ntreev.com/IUserService/OnUserItemsMoved")]
        void OnUserItemsMoved(Ntreev.Library.SignatureDate signatureDate, string[] itemPaths, string[] parentPaths);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://www.ntreev.com/IUserService/OnUserItemsDeleted")]
        void OnUserItemsDeleted(Ntreev.Library.SignatureDate signatureDate, string[] itemPaths);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://www.ntreev.com/IUserService/OnUsersLoggedIn")]
        void OnUsersLoggedIn(Ntreev.Library.SignatureDate signatureDate, string[] userIDs);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://www.ntreev.com/IUserService/OnUsersLoggedOut")]
        void OnUsersLoggedOut(Ntreev.Library.SignatureDate signatureDate, string[] userIDs);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://www.ntreev.com/IUserService/OnUsersKicked")]
        void OnUsersKicked(Ntreev.Library.SignatureDate signatureDate, string[] userIDs, string[] comments);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://www.ntreev.com/IUserService/OnUsersBanChanged")]
        void OnUsersBanChanged(Ntreev.Library.SignatureDate signatureDate, Ntreev.Crema.ServiceModel.BanInfo[] banInfos, Ntreev.Crema.ServiceModel.BanChangeType changeType, string[] comments);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://www.ntreev.com/IUserService/OnMessageReceived")]
        void OnMessageReceived(Ntreev.Library.SignatureDate signatureDate, string[] userIDs, string message, Ntreev.Crema.ServiceModel.MessageType messageType);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://www.ntreev.com/IUserService/OnMessageReceived2")]
        void OnMessageReceived2(Ntreev.Library.SignatureDate signatureDate, string[] userIDs, string message, Ntreev.Crema.ServiceModel.MessageType messageType, Ntreev.Crema.ServiceModel.NotifyMessageType nofiMessageType);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ntreev.com/IUserService/OnPing", ReplyAction="http://www.ntreev.com/IUserService/OnPingResponse")]
        bool OnPing();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    internal interface IUserServiceChannel : Ntreev.Crema.Services.UserService.IUserService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    internal partial class UserServiceClient : System.ServiceModel.DuplexClientBase<Ntreev.Crema.Services.UserService.IUserService>, Ntreev.Crema.Services.UserService.IUserService {
        
        public UserServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public UserServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public UserServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public UserServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public UserServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public Ntreev.Crema.ServiceModel.ResultBase<Ntreev.Crema.ServiceModel.UserContextMetaData> Subscribe(string userID, byte[] password, string version, string platformID, string culture) {
            return base.Channel.Subscribe(userID, password, version, platformID, culture);
        }
        
        public Ntreev.Crema.ServiceModel.ResultBase Unsubscribe() {
            return base.Channel.Unsubscribe();
        }
        
        public Ntreev.Crema.ServiceModel.ResultBase Shutdown(int milliseconds, Ntreev.Crema.ServiceModel.ShutdownType shutdownType, string message) {
            return base.Channel.Shutdown(milliseconds, shutdownType, message);
        }
        
        public Ntreev.Crema.ServiceModel.ResultBase CancelShutdown() {
            return base.Channel.CancelShutdown();
        }
        
        public Ntreev.Crema.ServiceModel.ResultBase<Ntreev.Crema.ServiceModel.UserInfo> NewUser(string userID, string categoryPath, byte[] password, string userName, Ntreev.Crema.ServiceModel.Authority authority) {
            return base.Channel.NewUser(userID, categoryPath, password, userName, authority);
        }
        
        public Ntreev.Crema.ServiceModel.ResultBase NewUserCategory(string categoryPath) {
            return base.Channel.NewUserCategory(categoryPath);
        }
        
        public Ntreev.Crema.ServiceModel.ResultBase RenameUserItem(string itemPath, string newName) {
            return base.Channel.RenameUserItem(itemPath, newName);
        }
        
        public Ntreev.Crema.ServiceModel.ResultBase MoveUserItem(string itemPath, string parentPath) {
            return base.Channel.MoveUserItem(itemPath, parentPath);
        }
        
        public Ntreev.Crema.ServiceModel.ResultBase DeleteUserItem(string itemPath) {
            return base.Channel.DeleteUserItem(itemPath);
        }
        
        public Ntreev.Crema.ServiceModel.ResultBase<Ntreev.Crema.ServiceModel.UserInfo> ChangeUserInfo(string userID, byte[] password, byte[] newPassword, string userName, System.Nullable<Ntreev.Crema.ServiceModel.Authority> authority) {
            return base.Channel.ChangeUserInfo(userID, password, newPassword, userName, authority);
        }
        
        public Ntreev.Crema.ServiceModel.ResultBase Kick(string userID, string comment) {
            return base.Channel.Kick(userID, comment);
        }
        
        public Ntreev.Crema.ServiceModel.ResultBase<Ntreev.Crema.ServiceModel.BanInfo> Ban(string userID, string comment) {
            return base.Channel.Ban(userID, comment);
        }
        
        public Ntreev.Crema.ServiceModel.ResultBase Unban(string userID) {
            return base.Channel.Unban(userID);
        }
        
        public Ntreev.Crema.ServiceModel.ResultBase SendMessage(string userID, string message) {
            return base.Channel.SendMessage(userID, message);
        }
        
        public Ntreev.Crema.ServiceModel.ResultBase NotifyMessage(string[] userIDs, string message) {
            return base.Channel.NotifyMessage(userIDs, message);
        }
        
        public bool IsAlive() {
            return base.Channel.IsAlive();
        }
    }
}
