﻿////////////////////////////////////////////////////////////////////////////
//
// Copyright 2016 Realm Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Realms.Sync.Exceptions;

namespace Realms.Sync
{
    /// <summary>
    /// Objects of this class allow to change permissions of owned Realms.
    /// They are created exclusively by the client and are processed by the server
    /// as indicated by the status fields.
    /// </summary>
    /// <remarks>
    /// <see cref="PermissionChange"/> objects allow to grant and revoke permissions by setting
    /// <see cref="MayRead" />, <see cref="MayWrite" /> and <see cref="MayManage" /> accordingly. 
    /// If any of these flags are not set, these are merged
    /// with either the existing or default permissions as applicable. As a
    /// side-effect this causes that the default permissions are permanently
    /// materialized for the affected Realm files and the affected user.
    /// Once the request has been processed, the <see cref="Status"/>, <see cref="StatusMessage"/>, and
    /// <see cref="ErrorCode"/> will be updated accordingly.
    /// </remarks>
    [Explicit]
    public class PermissionChange : RealmObject, IPermissionObject, IStatusObject
    {
        /// <inheritdoc />
        [PrimaryKey, Required]
        [MapTo("id")]
        public string Id { get; private set; } = Guid.NewGuid().ToString();

        /// <inheritdoc />
        [MapTo("createdAt")]
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

        /// <inheritdoc />
        [MapTo("updatedAt")]
        public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.UtcNow;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
        [MapTo("statusCode")]
        public int? StatusCode { get; set; }

        /// <inheritdoc />
        [MapTo("statusMessage")]
        public string StatusMessage { get; private set; }

        /// <inheritdoc />
        public ManagementObjectStatus Status
        {
            get
            {
                switch (StatusCode)
                {
                    case null:
                        return ManagementObjectStatus.NotProcessed;
                    case 0:
                        return ManagementObjectStatus.Success;
                    default:
                        return ManagementObjectStatus.Error;
                }
            }
        }

        /// <inheritdoc />
        public ErrorCode? ErrorCode => ErrorCodeHelper.GetErrorCode(StatusCode);

        /// <summary>
        /// Gets the user or users to effect.
        /// </summary>
        /// <value><c>*</c> to change the permissions for all users.</value>
        [Required]
        [MapTo("userId")]
        public string UserId { get; private set; }

        /// <summary>
        /// Gets the metadata key (if any) of the user(s) to effect.
        /// </summary>
        /// <value>A metadata key or null if the change is not based on metadata values.</value>
        [MapTo("metadataKey")]
        public string MetadataKey { get; private set; }

        /// <summary>
        /// Gets the metadata value (if any) of the user(s) to effect.
        /// </summary>
        /// <value>
        /// A value corresponding to <see cref="MetadataKey" /> or null if the change
        /// is not based on metadata values.
        /// </value>
        [MapTo("metadataValue")]
        public string MetadataValue { get; private set; }

        /// <summary>
        /// Gets the Realm to change permissions for.
        /// </summary>
        /// <value><c>*</c> to change the permissions of all Realms.</value>
        [Required]
        [MapTo("realmUrl")]
        public string RealmUrl { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the user(s) have read access to the specified Realm(s).
        /// </summary>
        /// <value><c>true</c> or <c>false</c> to request this new value. <c>null</c> to keep current value.</value>
        [MapTo("mayRead")]
        public bool? MayRead { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the user(s) have write access to the specified Realm(s).
        /// </summary>
        /// <value><c>true</c> or <c>false</c> to request this new value. <c>null</c> to keep current value.</value>
        [MapTo("mayWrite")]
        public bool? MayWrite { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the user(s) have manage access to the specified Realm(s).
        /// </summary>
        /// <value><c>true</c> or <c>false</c> to request this new value. <c>null</c> to keep current value.</value>
        [MapTo("mayManage")]
        public bool? MayManage { get; private set; }

        internal PermissionChange(string userId, string realmUrl, bool? mayRead = null, bool? mayWrite = null, bool? mayManage = null)
            : this(mayRead, mayWrite, mayManage)
        {
            UserId = userId;
            RealmUrl = realmUrl;
        }

        internal PermissionChange(string key, string value, string realmUrl, bool? mayRead = null, bool? mayWrite = null, bool? mayManage = null)
            : this(mayRead, mayWrite, mayManage)
        {
            MetadataKey = key;
            MetadataValue = value;
            UserId = string.Empty;
            RealmUrl = realmUrl;
        }

        private PermissionChange(bool? mayRead = null, bool? mayWrite = null, bool? mayManage = null)
        {
            MayRead = mayRead;
            MayWrite = mayWrite;
            MayManage = mayManage;
        }

        private PermissionChange()
        {
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(StatusCode))
            {
                RaisePropertyChanged(nameof(Status));
                RaisePropertyChanged(nameof(ErrorCode));
            }
        }
    }
}
