﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPM.Shell.Services.Model
{
    public class FolderVersionEntry
    {
        private readonly List<FileHistoryEntry> files = new List<FileHistoryEntry>();

        public FolderVersionEntry()
        {
            this.Timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public void AddEntry(string path, string hash, FileHistoryType editType)
            => files.Add(new FileHistoryEntry
            {
                Path = path,
                Hash = hash,
                EditType = editType
            });
        
        [JsonProperty("timestamp")]
        public int Timestamp { get; set; }
        [JsonProperty("files")]
        public IReadOnlyList<FileHistoryEntry> Files => files;
    }
}
