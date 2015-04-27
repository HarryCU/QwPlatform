using System;
using QwVirtualFileSystem.Cache.IO;
using QwVirtualFileSystem.Collections;

namespace QwVirtualFileSystem.IO
{
    internal class Folder : AbstractVFSItem, IFolder
    {
        public const string RootName = "VFS:";

        private readonly VBuffer _buffer;

        private readonly FileArray _files;
        private readonly FolderArray _folders;

        public VBuffer Buffer
        {
            get { return _buffer; }
        }

        public Folder(string fullName)
            : base(fullName)
        {
            _buffer = new VBuffer();
            _files = new FileArray();
            _folders = new FolderArray();
        }

        public IFolderArray SubFolders
        {
            get { return _folders; }
        }

        public IFileArray Files
        {
            get { return _files; }
        }

        public IStreamReader OpenRead(string fileName)
        {
            var file = SearchFile(fileName);
            if (file == null)
                throw new VFSRuntimeException("{0}文件不存在.", fileName);
            return file.OpenRead();
        }

        public IFile SearchFile(string fileName)
        {
            var folder = FildFolder(fileName, false);
            if (folder == null)
                throw new VFSRuntimeException("{0}文件所属文件夹不存在.", fileName);
            var name = VFSPath.GetFileName(fileName);
            return folder.FildFileByName(name);
        }

        private File FildFileByName(string name)
        {
            return _files.FindByName(name) as File;
        }

        private void AppendFile(File file)
        {
            _files.Add(file);
        }

        public void AppendFile(string fileName, byte[] buffer)
        {
            if (FullName.Equals(VFSPath.GetFolderPath(fileName), StringComparison.CurrentCultureIgnoreCase))
            {
                var bufferInfo = Buffer.ApplyFor(buffer);
                AppendFile(new File(bufferInfo, fileName, this));
                return;
            }
            var folder = FildFolder(fileName, true);
            if (folder != null)
            {
                folder.AppendFile(fileName, buffer);
            }
        }

        private Folder FildFolder(string fullPath, bool create)
        {
            var pathItem = VFSPath.Parse(fullPath);
            if (pathItem == null)
                throw new VFSRuntimeException("{0} 路径并非合法路径.", fullPath);

            var folder = FindFolder(pathItem, create);
            pathItem = pathItem.GetNext();
            while (pathItem != null && !pathItem.IsFileName)
            {
                folder = folder.FindFolder(pathItem, create);
                pathItem = pathItem.GetNext();
            }
            return folder;
        }

        private Folder FindFolder(VFSPathItem item, bool create = false)
        {
            if (Name.Equals(item.Name, StringComparison.CurrentCultureIgnoreCase))
                return this;

            var folder = _folders.FindByName(item.Name) as Folder;
            if (folder == null && create)
            {
                folder = Create(item.ToFolderPath());
                AddSubFolder(folder);
            }
            return folder;
        }

        public Folder AddSubFolder(Folder folder)
        {
            _folders.Add(folder);
            return folder;
        }

        public Folder AddSubFolder(string fullName)
        {
            return AddSubFolder(Create(fullName));
        }

        public static Folder CreateRoot()
        {
            return Create(RootName);
        }

        public static Folder Create(string fullName)
        {
            return new Folder(fullName);
        }
    }
}
