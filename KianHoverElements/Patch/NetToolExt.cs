using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ColossalFramework;
using ColossalFramework.Math;
using UnityEngine;
using static Kian.Mod.ShortCuts;
using static Kian.Skins.SkinManager;


namespace Kian.Patch {

    public class NetToolExt : NetTool {

        public static void RenderNode(ushort nodeID) {
            NetNode node = Node(nodeID);
            var seg0 = Segment(node.m_segment0);
            bool bStart = seg0.m_startNode == nodeID;
            var direction = bStart ? seg0.m_startDirection : seg0.m_endDirection;
            var position = node.m_position;
            RenderNode(nodeID, node.Info, position, direction);
        }
        public static void RenderNode(ushort nodeID, NetInfo info, Vector3 position, Vector3 direction) {
            if (info.m_nodes == null) {
                return;
            }
            NetManager instance = Singleton<NetManager>.instance;
            position.y += 0.15f;
            Quaternion identity = Quaternion.identity;
            float vscale = info.m_netAI.GetVScale();
            Vector3 b = new Vector3(direction.z, 0f, -direction.x) * info.m_halfWidth;
            Vector3 vector = position + b;
            Vector3 vector2 = position - b;
            Vector3 vector3 = vector2;
            Vector3 vector4 = vector;
            float d = Mathf.Min(info.m_halfWidth * 1.33333337f, 16f);
            Vector3 vector5 = vector - direction * d;
            Vector3 vector6 = vector3 - direction * d;
            Vector3 vector7 = vector2 - direction * d;
            Vector3 vector8 = vector4 - direction * d;
            Vector3 vector9 = vector + direction * d;
            Vector3 vector10 = vector3 + direction * d;
            Vector3 vector11 = vector2 + direction * d;
            Vector3 vector12 = vector4 + direction * d;
            Matrix4x4 value = NetSegment.CalculateControlMatrix(vector, vector5, vector6, vector3, vector, vector5, vector6, vector3, position, vscale);
            Matrix4x4 value2 = NetSegment.CalculateControlMatrix(vector2, vector11, vector12, vector4, vector2, vector11, vector12, vector4, position, vscale);
            Matrix4x4 value3 = NetSegment.CalculateControlMatrix(vector, vector9, vector10, vector3, vector, vector9, vector10, vector3, position, vscale);
            Matrix4x4 value4 = NetSegment.CalculateControlMatrix(vector2, vector7, vector8, vector4, vector2, vector7, vector8, vector4, position, vscale);
            value.SetRow(3, value.GetRow(3) + new Vector4(0.2f, 0.2f, 0.2f, 0.2f));
            value2.SetRow(3, value2.GetRow(3) + new Vector4(0.2f, 0.2f, 0.2f, 0.2f));
            value3.SetRow(3, value3.GetRow(3) + new Vector4(0.2f, 0.2f, 0.2f, 0.2f));
            value4.SetRow(3, value4.GetRow(3) + new Vector4(0.2f, 0.2f, 0.2f, 0.2f));
            Vector4 value5 = new Vector4(0.5f / info.m_halfWidth, 1f / info.m_segmentLength, 0.5f - info.m_pavementWidth / info.m_halfWidth * 0.5f, info.m_pavementWidth / info.m_halfWidth * 0.5f);
            Vector4 zero = Vector4.zero;
            zero.w = (value.m33 + value2.m33 + value3.m33 + value4.m33) * 0.25f;
            Vector4 value6 = new Vector4(info.m_pavementWidth / info.m_halfWidth * 0.5f, 1f, info.m_pavementWidth / info.m_halfWidth * 0.5f, 1f);
            instance.m_materialBlock.Clear();
            instance.m_materialBlock.SetMatrix(instance.ID_LeftMatrix, value);
            instance.m_materialBlock.SetMatrix(instance.ID_RightMatrix, value2);
            instance.m_materialBlock.SetMatrix(instance.ID_LeftMatrixB, value3);
            instance.m_materialBlock.SetMatrix(instance.ID_RightMatrixB, value4);
            instance.m_materialBlock.SetVector(instance.ID_MeshScale, value5);
            instance.m_materialBlock.SetVector(instance.ID_CenterPos, zero);
            instance.m_materialBlock.SetVector(instance.ID_SideScale, value6);
            instance.m_materialBlock.SetColor(instance.ID_Color, info.m_color);
            if (info.m_requireSurfaceMaps) {
                Texture texture;
                Texture value7;
                Vector4 value8;
                Singleton<TerrainManager>.instance.GetSurfaceMapping(position, out texture, out value7, out value8);
                if (texture != null) {
                    instance.m_materialBlock.SetTexture(instance.ID_SurfaceTexA, texture);
                    instance.m_materialBlock.SetTexture(instance.ID_SurfaceTexB, value7);
                    instance.m_materialBlock.SetVector(instance.ID_SurfaceMapping, value8);
                }
            }
            for (int i = 0; i < info.m_nodes.Length; i++) {
                NetInfo.Node node = info.m_nodes[i];
                if (node.CheckFlags(NetNode.Flags.None)) {
                    ToolManager instance2 = Singleton<ToolManager>.instance;
                    instance2.m_drawCallData.m_defaultCalls = instance2.m_drawCallData.m_defaultCalls + 1;
                    // TODO play with m_nodeMaterial and m_materialBlock
                    Graphics.DrawMesh(node.m_nodeMesh, position, identity, node.m_nodeMaterial, node.m_layer, null, 0, instance.m_materialBlock);
                }
            }
        }
    }
}