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

using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Commands;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Crema.Commands.Consoles
{
    public abstract class ConsoleCommandContextBase : CommandContextBase
    {
        private Authentication authentication;
        private Authority authority;

        private IConsoleDrive[] driveItems;
        private IConsoleDrive drive;
        private readonly Dictionary<IConsoleDrive, string> drivePaths = new Dictionary<IConsoleDrive, string>();
        private string path;
        private Authentication commission;

        protected ConsoleCommandContextBase(IEnumerable<IConsoleDrive> driveItems, IEnumerable<IConsoleCommand> commands, IEnumerable<IConsoleCommandProvider> commandProviders)
            : base(ConcatCommands(driveItems, commands), commandProviders.Select(item => item.CommandProvider))
        {
            this.Name = string.Empty;
            this.driveItems = driveItems.ToArray();
            foreach (var item in this.Commands)
            {
                if (item is ConsoleCommandBase command)
                {
                    command.CommandContext = this;
                }
                else if (item is ConsoleCommandMethodBase commandMethod)
                {
                    commandMethod.CommandContext = this;
                }
            }

            foreach (var item in commandProviders)
            {
                if (item is ConsoleCommandProviderBase provider)
                {
                    provider.CommandContext = this;
                }
            }
            foreach (var item in driveItems)
            {
                if (item is ConsoleDriveBase driveBase)
                {
                    driveBase.CommandContext = this;
                }
                drivePaths[item] = PathUtility.Separator;
            }
            this.drive = driveItems.Single(item => item is DataBasesConsoleDrive);
        }

        public static string[] GetAbsolutePath(string path, string relativePath)
        {
            var segments = StringUtility.Split(path, PathUtility.SeparatorChar);
            var items = StringUtility.Split(relativePath, PathUtility.SeparatorChar);
            var pathList = new List<string>();
            if (relativePath.First() != PathUtility.SeparatorChar)
            {
                pathList.AddRange(segments);
            }
            if (items.Any() == true)
            {
                FixPath(pathList, items, 0);
            }
            return pathList.ToArray();
        }

        public IConsoleDrive GetDrive(string path)
        {
            var uri = new Uri(path, UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri == true)
            {
                return this.driveItems.First(item => item.Name == uri.Scheme);
            }
            return this.Drive;
        }

        public bool ConfirmToDelete()
        {
            try
            {
                this.Terminal.IsCommandMode = false;
                return this.Terminal.ReadString("type 'delete' to delete:") == "delete";
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            finally
            {
                this.Terminal.IsCommandMode = true;
            }
        }

        public bool ReadYesOrNo(string title)
        {
            try
            {
                this.Terminal.IsCommandMode = false;
                return this.Terminal.ReadKey($"{title}(Y/N)", ConsoleKey.Y, ConsoleKey.N) == ConsoleKey.Y;
            }
            finally
            {
                this.Terminal.IsCommandMode = true;
            }
        }

        public override string ReadString(string title)
        {
            try
            {
                this.Terminal.IsCommandMode = false;
                return this.Terminal.ReadString(title);
            }
            finally
            {
                this.Terminal.IsCommandMode = true;
            }
        }

        public override SecureString ReadSecureString(string title)
        {
            try
            {
                this.Terminal.IsCommandMode = false;
                return this.Terminal.ReadSecureString(title);
            }
            finally
            {
                this.Terminal.IsCommandMode = true;
            }
        }

        public string GetAbsolutePath(string path)
        {
            if (NameValidator.VerifyCategoryPath(path) != false && NameValidator.VerifyItemPath(path) != false)
                path = this.path + path;

            var isDirectory = path.EndsWith(".") || path.EndsWith("..") || path.EndsWith(PathUtility.Separator);
            var segments = GetAbsolutePath(this.path, path);

            if (isDirectory == true)
                return CategoryName.Create(segments);
            return ItemName.Create(segments);
        }

        public string[] GetCompletion(string find)
        {
            return this.GetCompletion(find, false);
        }

        public string[] GetCompletion(string find, bool includeFiles)
        {
            try
            {
                var path = find == string.Empty ? this.Path : this.GetAbsolutePath(find);
                var allPath = this.drive.GetPaths();
                var level = StringUtility.Split(path, PathUtility.SeparatorChar).Length;
                var completionList = new List<string>();
                var findparent = Regex.Replace(find, "([^/]+$)", string.Empty);
                var parent = Regex.Replace(path, "([^/]+$)", string.Empty);
                foreach (var item in allPath)
                {
                    if (item.StartsWith(path) == false || item == path)
                        continue;
                    if (includeFiles == false && NameValidator.VerifyCategoryPath(item) == false)
                        continue;
                    var match = Regex.Match(item, $"^{parent}([^/]+)/?$");
                    if (match.Success == false)
                        continue;
                    completionList.Add(match.Groups[1].Value);
                }
                return completionList.Select(item => findparent + item).ToArray();
            }
            catch
            {
                return null;
            }
        }

        public void ChangeDirectory(Authentication authentication, string path)
        {
            var segments = GetAbsolutePath(this.path, path);
            var allPaths = this.drive.GetPaths();
            var absolutePath = segments.Any() ? string.Join(PathUtility.Separator, segments).WrapSeparator() : PathUtility.Separator;
            if (allPaths.Contains(absolutePath) == false)
                throw new ArgumentException($"No such directory : {absolutePath}");

            this.Update(authentication, segments, absolutePath);
            this.OnPathChanged(EventArgs.Empty);
        }

        public Authentication GetAuthentication(IConsoleCommand command)
        {
            if (this.commission != null)
                throw new Exception("임시 인증이 발급되어 있습니다.");
            this.commission = this.authentication.BeginCommission();
            return this.commission;
        }

        internal Authentication GetAuthenticationInternal(IConsoleCommand command)
        {
            return this.authentication;
        }

        public bool IsOnline
        {
            get { return this.authentication != null; }
        }

        public IConsoleDrive Drive
        {
            get { return this.drive; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (this.driveItems.Contains(value) == false)
                    throw new ItemNotFoundException(value.Name);
                this.drive = value;

                try
                {
                    this.Path = this.drivePaths[this.drive];
                }
                catch
                {
                    this.Path = PathUtility.Separator;
                    throw;
                }
            }
        }

        public IConsoleDrive[] DriveItems
        {
            get { return this.driveItems; }
        }

        public string Path
        {
            get
            {
                return this.path;
            }
            set
            {
                this.ChangeDirectory(this.authentication, value);
            }
        }

        public string Prompt
        {
            get
            {
                if (this.authentication == null)
                    return $"{this.Address}";

                if (this.Target == null)
                {
                    var path = this.Path;
                    if (path != PathUtility.Separator)
                        path = path.TrimEnd(PathUtility.SeparatorChar);
                    var prompt = $"{authentication.ID}@{this.Address}{path}";
                    if (this.Drive != null)
                        prompt = $"{this.Drive.Name}://{prompt}";
                    else
                        prompt = $"://{prompt}";
                    if (this.Target == null)
                        return prompt;
                    return prompt + "-" + this.Target;
                }
                else
                {
                    return $"{this.Target.GetType().Name}{Uri.SchemeDelimiter}{authentication.ID}@{this.Address}/{this.Target}";
                }
            }
        }

        public Authority Authority
        {
            get { return this.authority; }
        }

        public abstract ICremaHost CremaHost
        {
            get;
        }

        public object Target
        {
            get; set;
        }

        public abstract string Address
        {
            get;
        }

        public string UserID
        {
            get { return this.authentication.ID; }
        }

        public Guid Token => this.authentication.Token;

        public ConsoleTerminalBase Terminal { get; internal set; }

        public event EventHandler PathChanged;

        protected void Initialize(Authentication authentication)
        {
            this.authentication = authentication;
#if SERVER
            this.authentication.Expired += Authentication_Expired;
#endif
            this.authority = GetAuthority();
            this.path = PathUtility.Separator;

            Authority GetAuthority()
            {
                var userContext = this.CremaHost.GetService(typeof(IUserContext)) as IUserContext;
                return userContext.Dispatcher.Invoke(() => userContext.Users[authentication.ID].Authority);
            }
        }

        protected void Release()
        {
#if SERVER
            if (this.authentication != null)
            {
                this.authentication.Expired -= Authentication_Expired;
            }
#endif
            this.authentication = null;
            this.commission = null;
            this.OnPathChanged(EventArgs.Empty);
        }

        protected virtual void OnPathChanged(EventArgs e)
        {
            this.PathChanged?.Invoke(this, e);
        }

        protected override bool OnExecute(ICommand command, string arguments)
        {
            try
            {
                return base.OnExecute(command, arguments);
            }
            finally
            {
                if (this.commission != null)
                    this.authentication.EndCommission(this.commission);
                this.commission = null;
            }
        }

        private static string SecureStringToString(SecureString value)
        {
            var valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        private static void FixPath(IList<string> pathList, string[] names, int index)
        {
            var name = names[index];

            if (name == "..")
            {
                if (pathList.Any() == true)
                    pathList.RemoveAt(pathList.Count - 1);
                else
                    throw new InvalidOperationException("invalid path");
            }
            else if (name == ".")
            {

            }
            else
            {
                pathList.Add(name);
            }

            if (index + 1 < names.Length)
                FixPath(pathList, names, index + 1);
        }

        private static IEnumerable<ICommand> ConcatCommands(IEnumerable<IConsoleDrive> driveItems, IEnumerable<IConsoleCommand> commands)
        {
            var driveCommandList = new List<ICommand>(commands.Select(item => item.Command));
            foreach (var item in driveItems)
            {
                driveCommandList.Add(new ChangeDriveCommand(item));
            }
            return driveCommandList;
        }

        private void Authentication_Expired(object sender, EventArgs e)
        {
            this.Release();
        }

        private void Update(Authentication authentication, string[] segments, string path)
        {
            this.drive.SetPath(authentication, path);
            this.path = path;
            this.drivePaths[this.drive] = path;
        }

        internal static void Validate(SecureString value1, SecureString value2)
        {
            if (SecureStringToString(value1) != SecureStringToString(value2))
                throw new Exception("암호가 일치하지 않습니다.");
        }
    }
}