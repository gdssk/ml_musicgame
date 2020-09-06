// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019-present, Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Developer Agreement, located
// here: https://auth.magicleap.com/terms/developer
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if UNITY_EDITOR || PLATFORM_LUMIN

using System;
using MagicLeap.Core.StarterKit;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    /// <summary>
    /// メッシュ表示
    /// </summary>
    public class MyMeshingVisualizer : MonoBehaviour
    {
        /// <summary>
        /// MLSpatialMapper
        /// </summary>
        [SerializeField, Tooltip("The MLSpatialMapper from which to get update on mesh types.")]
        private MLSpatialMapper _mlSpatialMapper = null;

        /// <summary>
        /// Material
        /// </summary>
        [SerializeField, Tooltip("The material to apply for point cloud rendering.")]
        private Material _pointCloudMaterial = null;

        /// <summary>
        /// Awake
        /// </summary>
        void Awake()
        {
            if (_mlSpatialMapper == null)
            {
                Debug.LogError("Error: MeshingVisualizer._mlSpatialMapper is not set, disabling script!");
                enabled = false;
                return;
            }

            if (_pointCloudMaterial == null)
            {
                Debug.LogError("Error: MeshingVisualizer._pointCloudMaterial is not set, disabling script!");
                enabled = false;
                return;
            }
        }

        /// <summary>
        /// Start
        /// </summary>
        public void StartMeshHandle()
        {
            _mlSpatialMapper.meshAdded += HandleOnMeshReady;
            _mlSpatialMapper.meshUpdated += HandleOnMeshReady;
        }
        
        /// <summary>
        /// Stop
        /// </summary>
        public void StopMeshHandle()
        {
            _mlSpatialMapper.meshAdded -= HandleOnMeshReady;
            _mlSpatialMapper.meshUpdated -= HandleOnMeshReady;
        }

        /// <summary>
        /// Update
        /// </summary>
        void Update()
        {
            _mlSpatialMapper.gameObject.transform.position = Camera.main.transform.position;
        }

        /// <summary>
        /// DestroyMesh
        /// </summary>
        public void DestroyMesh()
        {
            _mlSpatialMapper.DestroyAllMeshes();
            _mlSpatialMapper.RefreshAllMeshes();
        }

        /// <summary>
        /// Updates the currently selected render material on the MeshRenderer.
        /// </summary>
        /// <param name="meshRenderer">The MeshRenderer that should be updated.</param>
        private void UpdateRenderer(MeshRenderer meshRenderer)
        {
            if (meshRenderer != null)
            {
                meshRenderer.enabled = true;
                meshRenderer.material = _pointCloudMaterial;
            }
        }

        #if UNITY_2019_3_OR_NEWER
        /// <summary>
        /// Handles the MeshReady event, which tracks and assigns the correct mesh renderer materials.
        /// </summary>
        /// <param name="meshId">Id of the mesh that got added / upated.</param>
        private void HandleOnMeshReady(UnityEngine.XR.MeshId meshId)
        {
            if (_mlSpatialMapper.meshIdToGameObjectMap.ContainsKey(meshId))
            {
                UpdateRenderer(_mlSpatialMapper.meshIdToGameObjectMap[meshId].GetComponent<MeshRenderer>());
            }
        }
        #else
        /// <summary>
        /// Handles the MeshReady event, which tracks and assigns the correct mesh renderer materials.
        /// </summary>
        /// <param name="meshId">Id of the mesh that got added / upated.</param>
        private void HandleOnMeshReady(TrackableId meshId)
        {
            if (_mlSpatialMapper.meshIdToGameObjectMap.ContainsKey(meshId))
            {
                UpdateRenderer(_mlSpatialMapper.meshIdToGameObjectMap[meshId].GetComponent<MeshRenderer>());
            }
        }
        #endif
    }
}
#endif
