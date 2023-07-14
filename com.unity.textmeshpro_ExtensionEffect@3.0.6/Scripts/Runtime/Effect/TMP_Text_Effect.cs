#define TMP_PRESENT

using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;
using System.Globalization;


namespace TMPro
{
    
    /// <summary>
    /// Base class which contains common properties and functions shared between the TextMeshPro and TextMeshProUGUI component.
    /// </summary>
    public abstract class TMP_Text_Effect : TMP_Text
    {

        #region 字体特效（描边，阴影）
        /// <summary>
        /// 设置字体的轮廓厚度
        /// </summary>
        public new float outlineWidth
        {
            get
            {
                return m_outlineWidth;
            }
            set { if (m_outlineWidth == value) return; m_havePropertiesChanged = true; checkPaddingRequired = true; m_outlineWidth = value; SetOutlineThickness(value); SetVerticesDirty(); }
        }

        public float faceDilate
        {
            get
            {
                return m_faceDilate;
            }
            set { if (m_faceDilate == value) return; m_havePropertiesChanged = true; checkPaddingRequired = true; m_faceDilate = value; SetOutlineThickness(value); SetVerticesDirty(); }
        }
        [SerializeField]
        protected float m_faceDilate = 0.0f;

        public float scaleRatioA
        {
            get
            {
                return m_scaleRatioA;
            }
            set
            {
                if (m_scaleRatioA == value)
                {
                    return;
                }
                m_scaleRatioA = value;
            }
        }
        protected float m_scaleRatioA = 1.0f;

        #region 投影
        public float underlayDilate
        {
            get
            {
                return m_underlayDilate;
            }
            set
            {
                if (m_underlayDilate == value)
                {
                    return;
                }
                checkPaddingRequired = true;
                m_havePropertiesChanged = true;
                m_underlayDilate = value;
                SetOutlineThickness(value);
                SetVerticesDirty();
            }
        }
        [SerializeField]
        protected float m_underlayDilate = 0f;

        public float underlayOffsetX
        {
            get
            {
                return m_underlayOffsetX;
            }
            set
            {
                float clamp = Mathf.Clamp(value, -1f, 1f);
                if (m_underlayOffsetX == clamp)
                {
                    return;
                }
                checkPaddingRequired = true;
                m_havePropertiesChanged = true;
                m_underlayOffsetX = clamp;
                SetOutlineThickness(clamp);
                SetVerticesDirty();
            }
        }
        [SerializeField]
        protected float m_underlayOffsetX = 0f;

        public float underlayOffsetY
        {
            get
            {
                return m_underlayOffsetY;
            }
            set
            {
                float clamp = Mathf.Clamp(value, -1f, 1f);
                if (m_underlayOffsetY == clamp)
                {
                    return;
                }
                checkPaddingRequired = true;
                m_havePropertiesChanged = true;
                m_underlayOffsetY = clamp;
                SetOutlineThickness(clamp);
                SetVerticesDirty();
            }
        }
        [SerializeField]
        protected float m_underlayOffsetY = 0f;

        public float scaleRatioC
        {
            get
            {
                return m_scaleRatioC;
            }
            set
            {
                if (m_scaleRatioC == value)
                {
                    return;
                }
                m_scaleRatioC = value;
            }
        }
        protected float m_scaleRatioC = 1.0f;
        #endregion

        #region 效果颜色
        private Quaternion m_rotation = Quaternion.identity;
        private Vector3 m_scale = Vector3.one;

        public Vector4 effectColor
        {
            get
            {
                return m_effectColor;
            }
            set
            {
                bool dirty = false;
                if (canvas == null) return;
                if (m_effectColor.Equals(value) == false)
                {
                    dirty = true;
                    m_effectColor = value;
                }
                if (m_rotation.Equals(transform.rotation) == false)
                {
                    dirty = true;
                    m_rotation = Quaternion.Inverse(canvas.transform.rotation)* transform.rotation;
                }
                if (m_scale.Equals(transform.lossyScale) == false)
                {
                    dirty = true;
                    m_scale.x = transform.lossyScale.x / canvas.transform.lossyScale.x;
                    m_scale.y = transform.lossyScale.y / canvas.transform.lossyScale.y;
                    m_scale.z = transform.lossyScale.z / canvas.transform.lossyScale.z;
                }
                if (dirty)
                {
                    m_havePropertiesChanged = true;
                    SetVerticesDirty();
                }
            }
        }
        [SerializeField]
        protected Vector4 m_effectColor = new Vector4(0, 0, 0, 1);

        protected Vector4 effectColorToTangent
        {
            get
            {
                Matrix4x4 matrix = new Matrix4x4();
                matrix.SetTRS(Vector3.zero, m_rotation, m_scale);
                Vector4 tangent = matrix.inverse * m_effectColor;
                return tangent;
            }
        }

        /// <summary>
        /// 刷新描边效果颜色
        /// </summary>
        public void SetEffectColorDirty()
        {
            bool dirty = false;
            if (m_rotation.Equals(transform.rotation) == false)
            {
                dirty = true;
                m_rotation = transform.rotation;
            }
            if (dirty)
            {
                m_havePropertiesChanged = true;
                SetVerticesDirty();
            }
        }

        #endregion
        #endregion

        protected override void FillCharacterVertexBuffers(int i, int index_X4)
        {
            int materialIndex = m_textInfo.characterInfo[i].materialReferenceIndex;
            index_X4 = m_textInfo.meshInfo[materialIndex].vertexCount;

            // Check to make sure our current mesh buffer allocations can hold these new Quads.
            if (index_X4 >= m_textInfo.meshInfo[materialIndex].vertices.Length)
                m_textInfo.meshInfo[materialIndex].ResizeMeshInfo(Mathf.NextPowerOfTwo((index_X4 + 4) / 4));


            TMP_CharacterInfo[] characterInfoArray = m_textInfo.characterInfo;
            m_textInfo.characterInfo[i].vertexIndex = index_X4;

            // Setup Vertices for Characters
            m_textInfo.meshInfo[materialIndex].vertices[0 + index_X4] = characterInfoArray[i].vertex_BL.position;
            m_textInfo.meshInfo[materialIndex].vertices[1 + index_X4] = characterInfoArray[i].vertex_TL.position;
            m_textInfo.meshInfo[materialIndex].vertices[2 + index_X4] = characterInfoArray[i].vertex_TR.position;
            m_textInfo.meshInfo[materialIndex].vertices[3 + index_X4] = characterInfoArray[i].vertex_BR.position;


            // Setup UVS0
            m_textInfo.meshInfo[materialIndex].uvs0[0 + index_X4] = characterInfoArray[i].vertex_BL.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[1 + index_X4] = characterInfoArray[i].vertex_TL.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[2 + index_X4] = characterInfoArray[i].vertex_TR.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[3 + index_X4] = characterInfoArray[i].vertex_BR.uv;


            // Setup UVS2
            m_textInfo.meshInfo[materialIndex].uvs2[0 + index_X4] = characterInfoArray[i].vertex_BL.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[1 + index_X4] = characterInfoArray[i].vertex_TL.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[2 + index_X4] = characterInfoArray[i].vertex_TR.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[3 + index_X4] = characterInfoArray[i].vertex_BR.uv2;

            // Setup UVS3
            m_textInfo.meshInfo[materialIndex].uvs3[0 + index_X4] = characterInfoArray[i].vertex_BL.uv3;
            m_textInfo.meshInfo[materialIndex].uvs3[1 + index_X4] = characterInfoArray[i].vertex_TL.uv3;
            m_textInfo.meshInfo[materialIndex].uvs3[2 + index_X4] = characterInfoArray[i].vertex_TR.uv3;
            m_textInfo.meshInfo[materialIndex].uvs3[3 + index_X4] = characterInfoArray[i].vertex_BR.uv3;

            // Setup UVS4
            m_textInfo.meshInfo[materialIndex].uvs4[0 + index_X4] = characterInfoArray[i].vertex_BL.uv4;
            m_textInfo.meshInfo[materialIndex].uvs4[1 + index_X4] = characterInfoArray[i].vertex_TL.uv4;
            m_textInfo.meshInfo[materialIndex].uvs4[2 + index_X4] = characterInfoArray[i].vertex_TR.uv4;
            m_textInfo.meshInfo[materialIndex].uvs4[3 + index_X4] = characterInfoArray[i].vertex_BR.uv4;


            // setup Vertex Colors
            m_textInfo.meshInfo[materialIndex].colors32[0 + index_X4] = characterInfoArray[i].vertex_BL.color;
            m_textInfo.meshInfo[materialIndex].colors32[1 + index_X4] = characterInfoArray[i].vertex_TL.color;
            m_textInfo.meshInfo[materialIndex].colors32[2 + index_X4] = characterInfoArray[i].vertex_TR.color;
            m_textInfo.meshInfo[materialIndex].colors32[3 + index_X4] = characterInfoArray[i].vertex_BR.color;

            // steup Tangents
            m_textInfo.meshInfo[materialIndex].tangents[0 + index_X4] = characterInfoArray[i].vertex_BL.tangent;
            m_textInfo.meshInfo[materialIndex].tangents[1 + index_X4] = characterInfoArray[i].vertex_TL.tangent;
            m_textInfo.meshInfo[materialIndex].tangents[2 + index_X4] = characterInfoArray[i].vertex_TR.tangent;
            m_textInfo.meshInfo[materialIndex].tangents[3 + index_X4] = characterInfoArray[i].vertex_BR.tangent;

            m_textInfo.meshInfo[materialIndex].vertexCount = index_X4 + 4;
        }
        protected override void FillCharacterVertexBuffers(int i, int index_X4, bool isVolumetric)
        {
            int materialIndex = m_textInfo.characterInfo[i].materialReferenceIndex;
            index_X4 = m_textInfo.meshInfo[materialIndex].vertexCount;

            // Check to make sure our current mesh buffer allocations can hold these new Quads.
            if (index_X4 >= m_textInfo.meshInfo[materialIndex].vertices.Length)
                m_textInfo.meshInfo[materialIndex].ResizeMeshInfo(Mathf.NextPowerOfTwo((index_X4 + (isVolumetric ? 8 : 4)) / 4));

            TMP_CharacterInfo[] characterInfoArray = m_textInfo.characterInfo;
            m_textInfo.characterInfo[i].vertexIndex = index_X4;

            // Setup Vertices for Characters
            m_textInfo.meshInfo[materialIndex].vertices[0 + index_X4] = characterInfoArray[i].vertex_BL.position;
            m_textInfo.meshInfo[materialIndex].vertices[1 + index_X4] = characterInfoArray[i].vertex_TL.position;
            m_textInfo.meshInfo[materialIndex].vertices[2 + index_X4] = characterInfoArray[i].vertex_TR.position;
            m_textInfo.meshInfo[materialIndex].vertices[3 + index_X4] = characterInfoArray[i].vertex_BR.position;

            //if (isVolumetric)
            //{
            //    Vector3 depth = new Vector3(0, 0, m_fontSize * m_fontScale);
            //    m_textInfo.meshInfo[materialIndex].vertices[4 + index_X4] = characterInfoArray[i].vertex_BL.position + depth;
            //    m_textInfo.meshInfo[materialIndex].vertices[5 + index_X4] = characterInfoArray[i].vertex_TL.position + depth;
            //    m_textInfo.meshInfo[materialIndex].vertices[6 + index_X4] = characterInfoArray[i].vertex_TR.position + depth;
            //    m_textInfo.meshInfo[materialIndex].vertices[7 + index_X4] = characterInfoArray[i].vertex_BR.position + depth;
            //}

            // Setup UVS0
            m_textInfo.meshInfo[materialIndex].uvs0[0 + index_X4] = characterInfoArray[i].vertex_BL.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[1 + index_X4] = characterInfoArray[i].vertex_TL.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[2 + index_X4] = characterInfoArray[i].vertex_TR.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[3 + index_X4] = characterInfoArray[i].vertex_BR.uv;

            if (isVolumetric)
            {
                m_textInfo.meshInfo[materialIndex].uvs0[4 + index_X4] = characterInfoArray[i].vertex_BL.uv;
                m_textInfo.meshInfo[materialIndex].uvs0[5 + index_X4] = characterInfoArray[i].vertex_TL.uv;
                m_textInfo.meshInfo[materialIndex].uvs0[6 + index_X4] = characterInfoArray[i].vertex_TR.uv;
                m_textInfo.meshInfo[materialIndex].uvs0[7 + index_X4] = characterInfoArray[i].vertex_BR.uv;
            }


            // Setup UVS2
            m_textInfo.meshInfo[materialIndex].uvs2[0 + index_X4] = characterInfoArray[i].vertex_BL.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[1 + index_X4] = characterInfoArray[i].vertex_TL.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[2 + index_X4] = characterInfoArray[i].vertex_TR.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[3 + index_X4] = characterInfoArray[i].vertex_BR.uv2;

            if (isVolumetric)
            {
                m_textInfo.meshInfo[materialIndex].uvs2[4 + index_X4] = characterInfoArray[i].vertex_BL.uv2;
                m_textInfo.meshInfo[materialIndex].uvs2[5 + index_X4] = characterInfoArray[i].vertex_TL.uv2;
                m_textInfo.meshInfo[materialIndex].uvs2[6 + index_X4] = characterInfoArray[i].vertex_TR.uv2;
                m_textInfo.meshInfo[materialIndex].uvs2[7 + index_X4] = characterInfoArray[i].vertex_BR.uv2;
            }

            // Setup UVS3
            m_textInfo.meshInfo[materialIndex].uvs3[0 + index_X4] = characterInfoArray[i].vertex_BL.uv3;
            m_textInfo.meshInfo[materialIndex].uvs3[1 + index_X4] = characterInfoArray[i].vertex_TL.uv3;
            m_textInfo.meshInfo[materialIndex].uvs3[2 + index_X4] = characterInfoArray[i].vertex_TR.uv3;
            m_textInfo.meshInfo[materialIndex].uvs3[3 + index_X4] = characterInfoArray[i].vertex_BR.uv3;

            if (isVolumetric)
            {
                m_textInfo.meshInfo[materialIndex].uvs3[4 + index_X4] = characterInfoArray[i].vertex_BL.uv3;
                m_textInfo.meshInfo[materialIndex].uvs3[5 + index_X4] = characterInfoArray[i].vertex_TL.uv3;
                m_textInfo.meshInfo[materialIndex].uvs3[6 + index_X4] = characterInfoArray[i].vertex_TR.uv3;
                m_textInfo.meshInfo[materialIndex].uvs3[7 + index_X4] = characterInfoArray[i].vertex_BR.uv3;
            }

            // Setup UVS4
            m_textInfo.meshInfo[materialIndex].uvs4[0 + index_X4] = characterInfoArray[i].vertex_BL.uv4;
            m_textInfo.meshInfo[materialIndex].uvs4[1 + index_X4] = characterInfoArray[i].vertex_TL.uv4;
            m_textInfo.meshInfo[materialIndex].uvs4[2 + index_X4] = characterInfoArray[i].vertex_TR.uv4;
            m_textInfo.meshInfo[materialIndex].uvs4[3 + index_X4] = characterInfoArray[i].vertex_BR.uv4;

            if (isVolumetric)
            {
                m_textInfo.meshInfo[materialIndex].uvs4[4 + index_X4] = characterInfoArray[i].vertex_BL.uv4;
                m_textInfo.meshInfo[materialIndex].uvs4[5 + index_X4] = characterInfoArray[i].vertex_TL.uv4;
                m_textInfo.meshInfo[materialIndex].uvs4[6 + index_X4] = characterInfoArray[i].vertex_TR.uv4;
                m_textInfo.meshInfo[materialIndex].uvs4[7 + index_X4] = characterInfoArray[i].vertex_BR.uv4;
            }


            // setup Vertex Colors
            m_textInfo.meshInfo[materialIndex].colors32[0 + index_X4] = characterInfoArray[i].vertex_BL.color;
            m_textInfo.meshInfo[materialIndex].colors32[1 + index_X4] = characterInfoArray[i].vertex_TL.color;
            m_textInfo.meshInfo[materialIndex].colors32[2 + index_X4] = characterInfoArray[i].vertex_TR.color;
            m_textInfo.meshInfo[materialIndex].colors32[3 + index_X4] = characterInfoArray[i].vertex_BR.color;

            if (isVolumetric)
            {
                Color32 backColor = new Color32(255, 255, 128, 255);
                m_textInfo.meshInfo[materialIndex].colors32[4 + index_X4] = backColor; //characterInfoArray[i].vertex_BL.color;
                m_textInfo.meshInfo[materialIndex].colors32[5 + index_X4] = backColor; //characterInfoArray[i].vertex_TL.color;
                m_textInfo.meshInfo[materialIndex].colors32[6 + index_X4] = backColor; //characterInfoArray[i].vertex_TR.color;
                m_textInfo.meshInfo[materialIndex].colors32[7 + index_X4] = backColor; //characterInfoArray[i].vertex_BR.color;
            }


            // Setup Tangents
            m_textInfo.meshInfo[materialIndex].tangents[0 + index_X4] = characterInfoArray[i].vertex_BL.tangent;
            m_textInfo.meshInfo[materialIndex].tangents[1 + index_X4] = characterInfoArray[i].vertex_TL.tangent;
            m_textInfo.meshInfo[materialIndex].tangents[2 + index_X4] = characterInfoArray[i].vertex_TR.tangent;
            m_textInfo.meshInfo[materialIndex].tangents[3 + index_X4] = characterInfoArray[i].vertex_BR.tangent;

            // 体积的tangents可能有其他用途
            if (isVolumetric)
            {
                m_textInfo.meshInfo[materialIndex].tangents[4 + index_X4] = characterInfoArray[i].vertex_BL.tangent;
                m_textInfo.meshInfo[materialIndex].tangents[5 + index_X4] = characterInfoArray[i].vertex_TL.tangent;
                m_textInfo.meshInfo[materialIndex].tangents[6 + index_X4] = characterInfoArray[i].vertex_TR.tangent;
                m_textInfo.meshInfo[materialIndex].tangents[7 + index_X4] = characterInfoArray[i].vertex_BR.tangent;
            }

            m_textInfo.meshInfo[materialIndex].vertexCount = index_X4 + (!isVolumetric ? 4 : 8);
        }

        protected override void FillSpriteVertexBuffers(int i, int index_X4)
        {
            int materialIndex = m_textInfo.characterInfo[i].materialReferenceIndex;
            index_X4 = m_textInfo.meshInfo[materialIndex].vertexCount;

            // Check to make sure our current mesh buffer allocations can hold these new Quads.
            if (index_X4 >= m_textInfo.meshInfo[materialIndex].vertices.Length)
                m_textInfo.meshInfo[materialIndex].ResizeMeshInfo(Mathf.NextPowerOfTwo((index_X4 + 4) / 4));

            TMP_CharacterInfo[] characterInfoArray = m_textInfo.characterInfo;
            m_textInfo.characterInfo[i].vertexIndex = index_X4;

            // Setup Vertices for Characters
            m_textInfo.meshInfo[materialIndex].vertices[0 + index_X4] = characterInfoArray[i].vertex_BL.position;
            m_textInfo.meshInfo[materialIndex].vertices[1 + index_X4] = characterInfoArray[i].vertex_TL.position;
            m_textInfo.meshInfo[materialIndex].vertices[2 + index_X4] = characterInfoArray[i].vertex_TR.position;
            m_textInfo.meshInfo[materialIndex].vertices[3 + index_X4] = characterInfoArray[i].vertex_BR.position;


            // Setup UVS0
            m_textInfo.meshInfo[materialIndex].uvs0[0 + index_X4] = characterInfoArray[i].vertex_BL.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[1 + index_X4] = characterInfoArray[i].vertex_TL.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[2 + index_X4] = characterInfoArray[i].vertex_TR.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[3 + index_X4] = characterInfoArray[i].vertex_BR.uv;


            // Setup UVS2
            m_textInfo.meshInfo[materialIndex].uvs2[0 + index_X4] = characterInfoArray[i].vertex_BL.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[1 + index_X4] = characterInfoArray[i].vertex_TL.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[2 + index_X4] = characterInfoArray[i].vertex_TR.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[3 + index_X4] = characterInfoArray[i].vertex_BR.uv2;

            // Setup UVS3
            m_textInfo.meshInfo[materialIndex].uvs3[0 + index_X4] = characterInfoArray[i].vertex_BL.uv3;
            m_textInfo.meshInfo[materialIndex].uvs3[1 + index_X4] = characterInfoArray[i].vertex_TL.uv3;
            m_textInfo.meshInfo[materialIndex].uvs3[2 + index_X4] = characterInfoArray[i].vertex_TR.uv3;
            m_textInfo.meshInfo[materialIndex].uvs3[3 + index_X4] = characterInfoArray[i].vertex_BR.uv3;

            // Setup UVS4
            m_textInfo.meshInfo[materialIndex].uvs4[0 + index_X4] = characterInfoArray[i].vertex_BL.uv4;
            m_textInfo.meshInfo[materialIndex].uvs4[1 + index_X4] = characterInfoArray[i].vertex_TL.uv4;
            m_textInfo.meshInfo[materialIndex].uvs4[2 + index_X4] = characterInfoArray[i].vertex_TR.uv4;
            m_textInfo.meshInfo[materialIndex].uvs4[3 + index_X4] = characterInfoArray[i].vertex_BR.uv4;


            // setup Vertex Colors
            m_textInfo.meshInfo[materialIndex].colors32[0 + index_X4] = characterInfoArray[i].vertex_BL.color;
            m_textInfo.meshInfo[materialIndex].colors32[1 + index_X4] = characterInfoArray[i].vertex_TL.color;
            m_textInfo.meshInfo[materialIndex].colors32[2 + index_X4] = characterInfoArray[i].vertex_TR.color;
            m_textInfo.meshInfo[materialIndex].colors32[3 + index_X4] = characterInfoArray[i].vertex_BR.color;

            // setup Tangents
            m_textInfo.meshInfo[materialIndex].tangents[0 + index_X4] = characterInfoArray[i].vertex_BL.tangent;
            m_textInfo.meshInfo[materialIndex].tangents[1 + index_X4] = characterInfoArray[i].vertex_TL.tangent;
            m_textInfo.meshInfo[materialIndex].tangents[2 + index_X4] = characterInfoArray[i].vertex_TR.tangent;
            m_textInfo.meshInfo[materialIndex].tangents[3 + index_X4] = characterInfoArray[i].vertex_BR.tangent;

            m_textInfo.meshInfo[materialIndex].vertexCount = index_X4 + 4;
        }
    }
}
