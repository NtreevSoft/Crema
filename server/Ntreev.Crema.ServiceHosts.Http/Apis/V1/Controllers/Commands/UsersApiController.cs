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

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Ntreev.Crema.ServiceHosts.Http.Apis.V1.Requests.Commands;
using Ntreev.Crema.ServiceHosts.Http.Apis.V1.Responses.Commands;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.V1.Controllers.Commands
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/v1/commands")]
    public class UsersApiController : CremaApiController
    {

        [ImportingConstructor]
        public UsersApiController(ICremaHost cremaHost) : base(cremaHost)
        {
        }

        [HttpGet]
        [Route("users")]
        [AllowAnonymous]
        public string[] GetUserList()
        {
            return this.userContext.Dispatcher.Invoke(() =>
            {
                return this.userContext.Users.Select(item => item.ID).ToArray();
            });
        }

        [HttpPost]
        [Route("users/notify")]
        public void Notify(NotifyRequest request)
        {
            var authentication = this.Authentication;
            this.userContext.Dispatcher.Invoke(() =>
            {
                this.userContext.NotifyMessage(authentication, request.Message);
            });
        }

        [HttpGet]
        [Route("users/me")]
        public GetCurrentUserResponse GetCurrentUser()
        {
            return new GetCurrentUserResponse
            {
                UserId = this.Authentication.ID
            };
        }

        [HttpPost]
        [Route("users/{userId}/send-message")]
        public void SendMessage(string userId, [FromBody] SendMessageRequest request)
        {
            var user = this.GetUser(userId);
            var authentication = this.Authentication;
            user.Dispatcher.Invoke(() =>
            {
                user.SendMessage(authentication, request.Message);
            });
        }

        [HttpGet]
        [Route("users/{userId}/state")]
        [AllowAnonymous]
        public GetUserStateResponse GetUserState(string userId)
        {
            var user = this.GetUser(userId);
            return user.Dispatcher.Invoke(() => new GetUserStateResponse
            {
                UserState = user.UserState
            });
        }

        [HttpPost]
        [Route("users/{userId}/ban")]
        public void BanUser(string userId, [FromBody] BanUserRequest request)
        {
            var user = this.GetUser(userId);
            var authentication = this.Authentication;
            user.Dispatcher.Invoke(() =>
            {
                user.Ban(authentication, request.Comment);
            });
        }

        [HttpPut]
        [Route("users/{userId}/unban")]
        public void UnbanUser(string userId)
        {
            var user = this.GetUser(userId);
            var authentication = this.Authentication;
            user.Dispatcher.Invoke(() =>
            {
                user.Unban(authentication);
            });
        }

        [HttpGet]
        [Route("users/{userId}/ban-info")]
        [AllowAnonymous]
        public GetUserBanInfoResponse GetUserBanInfo(string userId)
        {
            var user = this.GetUser(userId);
            return user.Dispatcher.Invoke(() =>
            {
                var banInfo = user.BanInfo;
                return new GetUserBanInfoResponse
                {
                    UserId = banInfo.UserID,
                    Path = banInfo.Path,
                    Comment = banInfo.Comment,
                    DateTime = banInfo.DateTime
                };
            });
        }

        [HttpGet]
        [Route("users/{userId}/banned")]
        [AllowAnonymous]
        public IsUserBannedResponse IsUserBanned(string userId)
        {
            var user = this.GetUser(userId);
            var banned = user.Dispatcher.Invoke(() => user.BanInfo.Path != string.Empty);
            return new IsUserBannedResponse
            {
                Banned = banned
            };
        }

        [HttpGet]
        [Route("users/{userId}/is-online")]
        [AllowAnonymous]
        public IsUserOnlineResponse IsUserOnline(string userId)
        {
            var user = this.GetUser(userId);
            var isOnline = user.Dispatcher.Invoke(() => user.UserState == UserState.Online);
            return new IsUserOnlineResponse
            {
                IsOnline = isOnline
            };
        }

        [HttpPost]
        [Route("users/{userId}/kick")]
        public void KickUser(string userId, [FromBody] KickUserRequest request)
        {
            var user = this.GetUser(userId);
            var authentication = this.Authentication;
            user.Dispatcher.Invoke(() =>
            {
                user.Kick(authentication, request.Comment);
            });
        }

        [HttpGet]
        [Route("users/{userId}/contains")]
        [AllowAnonymous]
        public ContainsUserResponse ContainsUser(string userId)
        {
            var contains = userContext.Dispatcher.Invoke(() => userContext.Users.Contains(userId));
            return new ContainsUserResponse
            {
                Contains = contains
            };
        }

        [HttpGet]
        [Route("user-item")]
        [AllowAnonymous]
        public string[] GetUserItemList()
        {
            return this.userContext.Dispatcher.Invoke(() =>
            {
                return this.userContext.Select(item => item.Path).ToArray();
            });
        }

        [HttpPost]
        [Route("user-item/contains")]
        [AllowAnonymous]
        public ContainsUserItemResponse ContainsUserItem([FromBody] ContainsUserItemRequest request)
        {
            var contains = userContext.Dispatcher.Invoke(() => userContext.Contains(request.UserItemPath));
            return new ContainsUserItemResponse
            {
                Contains = contains
            };
        }

        [HttpPut]
        [Route("user-item/move")]
        public void MoveUserItem([FromBody] MoveUserItemRequest request)
        {
            var userItem = this.GetUserItem(request.UserItemPath);
            var authentication = this.Authentication;
            userItem.Dispatcher.Invoke(() =>
            {
                userItem.Move(authentication, request.ParentPath);
            });
        }

        [HttpPut]
        [Route("user-item/rename")]
        public void RenameUserItem([FromBody] RenameUserItemRequest request)
        {
            var userItem = this.GetUserItem(request.UserItemPath);
            var authentication = this.Authentication;
            userItem.Dispatcher.Invoke(() =>
            {
                userItem.Rename(authentication, request.NewName);
            });
        }

        [HttpPut]
        [Route("user-item/delete")]
        public void DeleteUserItem([FromBody] DeleteUserItemRequest request)
        {
            var userItem = this.GetUserItem(request.UserItemPath);
            var authentication = this.Authentication;
            userItem.Dispatcher.Invoke(() =>
            {
                userItem.Delete(authentication);
            });
        }

        private IUser GetUser(string userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            var user = this.userContext.Dispatcher.Invoke(() => this.userContext.Users[userId]);
            if (user == null)
                throw new UserNotFoundException(userId);

            return user;
        }

        private IUserItem GetUserItem(string userItemPath)
        {
            if (userItemPath == null)
                throw new ArgumentNullException(nameof(userItemPath));

            var userItem = this.userContext.Dispatcher.Invoke(() => this.userContext[userItemPath]);
            if (userItem == null)
                throw new ItemNotFoundException(userItemPath);

            return userItem;
        }
    }
}
