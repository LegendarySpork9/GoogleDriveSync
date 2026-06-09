// Copyright © - 14/05/2025 - Toby Hunter
using System;
using System.Collections.Generic;

namespace GoogleDriveSync.Models
{
    /// <summary>
    /// Stores the information about the file.
    /// </summary>
    public class FileModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string PathIds { get; set; }
        public string Path { get; set; }
        public bool Hidden { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public List<ChangeModel> Changes { get; set; } = new List<ChangeModel>();
    }
}
