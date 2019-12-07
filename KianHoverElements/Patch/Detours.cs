using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;
using static Kian.Mod.ShortCuts;
using static Kian.Skins.SkinManager.Util;
using System;

namespace Kian.Patch {
    public class Detours {


        public Color GetSegmentColor(ushort segmentID, ref global::NetSegment data, InfoManager.InfoMode infoMode) {
            RoadBaseAI netAI = Segment(segmentID).Info.m_netAI as RoadBaseAI; //this


        }

        public Color GetNodeColor(ushort nodeID, ref global::NetNode data, InfoManager.InfoMode infoMode) {
            RoadBaseAI netAI = Node(nodeID).Info.m_netAI as RoadBaseAI;//this

        }

        /*private void NetSegment.RenderInstance(
         * RenderManager.CameraInfo cameraInfo,
         * ushort segmentID,
         * int layerMask,
         * NetInfo info,
         * ref RenderManager.Instance data) */
        public void RenderInstance(
            RenderManager.CameraInfo cameraInfo,
            ushort segmentID,
            int layerMask,
            NetInfo info,
            ref RenderManager.Instance data) {
            NetSegment thisSegment = Segment(segmentID); //this

            NetManager instance = Singleton<NetManager>.instance;
            if (data.m_dirty) {
                data.m_dirty = false;
                Vector3 position = instance.m_nodes.m_buffer[(int)thisSegment.m_startNode].m_position;
                Vector3 position2 = instance.m_nodes.m_buffer[(int)thisSegment.m_endNode].m_position;
                data.m_position = (position + position2) * 0.5f;
                data.m_rotation = Quaternion.identity;
                data.m_dataColor0 = SegColor(segmentID, info.m_color);
                data.m_dataColor0.a = 0f;
                data.m_dataFloat0 = Singleton<WeatherManager>.instance.GetWindSpeed(data.m_position);
                data.m_dataVector0 = new Vector4(0.5f / info.m_halfWidth, 1f / info.m_segmentLength, 1f, 1f);
                Vector4 colorLocation = RenderManager.GetColorLocation((uint)(49152 + segmentID));
                Vector4 vector = colorLocation;
                if (NetNode.BlendJunction(thisSegment.m_startNode)) {
                    colorLocation = RenderManager.GetColorLocation(86016u + (uint)thisSegment.m_startNode);
                }
                if (NetNode.BlendJunction(thisSegment.m_endNode)) {
                    vector = RenderManager.GetColorLocation(86016u + (uint)thisSegment.m_endNode);
                }
                data.m_dataVector3 = new Vector4(colorLocation.x, colorLocation.y, vector.x, vector.y);
                if (info.m_segments == null || info.m_segments.Length == 0) {
                    if (info.m_lanes != null) {
                        bool invert;
                        if ((thisSegment.m_flags & NetSegment.Flags.Invert) != NetSegment.Flags.None) {
                            invert = true;
                            NetInfo info2 = instance.m_nodes.m_buffer[(int)thisSegment.m_endNode].Info;
                            NetNode.Flags flags;
                            Color color;
                            info2.m_netAI.GetNodeState(thisSegment.m_endNode, ref instance.m_nodes.m_buffer[(int)thisSegment.m_endNode], segmentID, ref thisSegment, out flags, out color);
                            NetInfo info3 = instance.m_nodes.m_buffer[(int)thisSegment.m_startNode].Info;
                            NetNode.Flags flags2;
                            Color color2;
                            info3.m_netAI.GetNodeState(thisSegment.m_startNode, ref instance.m_nodes.m_buffer[(int)thisSegment.m_startNode], segmentID, ref thisSegment, out flags2, out color2);
                        } else {
                            invert = false;
                            NetInfo info4 = instance.m_nodes.m_buffer[(int)thisSegment.m_startNode].Info;
                            NetNode.Flags flags;
                            Color color;
                            info4.m_netAI.GetNodeState(thisSegment.m_startNode, ref instance.m_nodes.m_buffer[(int)thisSegment.m_startNode], segmentID, ref thisSegment, out flags, out color);
                            NetInfo info5 = instance.m_nodes.m_buffer[(int)thisSegment.m_endNode].Info;
                            NetNode.Flags flags2;
                            Color color2;
                            info5.m_netAI.GetNodeState(thisSegment.m_endNode, ref instance.m_nodes.m_buffer[(int)thisSegment.m_endNode], segmentID, ref thisSegment, out flags2, out color2);
                        }
                        float startAngle = (float)thisSegment.m_cornerAngleStart * 0.0245436933f;
                        float endAngle = (float)thisSegment.m_cornerAngleEnd * 0.0245436933f;
                        int num = 0;
                        uint num2 = thisSegment.m_lanes;
                        int num3 = 0;
                        while (num3 < info.m_lanes.Length && num2 != 0u) {
                            instance.m_lanes.m_buffer[(int)((UIntPtr)num2)].RefreshInstance(num2, info.m_lanes[num3], startAngle, endAngle, invert, ref data, ref num);
                            num2 = instance.m_lanes.m_buffer[(int)((UIntPtr)num2)].m_nextLane;
                            num3++;
                        }
                    }
                } else {
                    float vscale = info.m_netAI.GetVScale();
                    Vector3 vector2;
                    Vector3 startDir;
                    bool smoothStart;
                    thisSegment.CalculateCorner(segmentID, true, true, true, out vector2, out startDir, out smoothStart);
                    Vector3 vector3;
                    Vector3 endDir;
                    bool smoothEnd;
                    thisSegment.CalculateCorner(segmentID, true, false, true, out vector3, out endDir, out smoothEnd);
                    Vector3 vector4;
                    Vector3 startDir2;
                    thisSegment.CalculateCorner(segmentID, true, true, false, out vector4, out startDir2, out smoothStart);
                    Vector3 vector5;
                    Vector3 endDir2;
                    thisSegment.CalculateCorner(segmentID, true, false, false, out vector5, out endDir2, out smoothEnd);
                    Vector3 vector6;
                    Vector3 vector7;
                    NetSegment.CalculateMiddlePoints(vector2, startDir, vector5, endDir2, smoothStart, smoothEnd, out vector6, out vector7);
                    Vector3 vector8;
                    Vector3 vector9;
                    NetSegment.CalculateMiddlePoints(vector4, startDir2, vector3, endDir, smoothStart, smoothEnd, out vector8, out vector9);
                    data.m_dataMatrix0 = NetSegment.CalculateControlMatrix(vector2, vector6, vector7, vector5, vector4, vector8, vector9, vector3, data.m_position, vscale);
                    data.m_dataMatrix1 = NetSegment.CalculateControlMatrix(vector4, vector8, vector9, vector3, vector2, vector6, vector7, vector5, data.m_position, vscale);
                }
                if ((thisSegment.m_flags & NetSegment.Flags.NameVisible2) != NetSegment.Flags.None) {
                    string segmentName = instance.GetSegmentName(segmentID);
                    UIFont nameFont = instance.m_properties.m_nameFont;
                    data.m_nameData = Singleton<InstanceManager>.instance.GetNameData(segmentName, nameFont, true);
                    if (data.m_nameData != null) {
                        float snapElevation = info.m_netAI.GetSnapElevation();
                        position.y += snapElevation;
                        position2.y += snapElevation;
                        Vector3 middlePos;
                        Vector3 middlePos2;
                        NetSegment.CalculateMiddlePoints(position, thisSegment.m_startDirection, position2, thisSegment.m_endDirection, true, true, out middlePos, out middlePos2);
                        data.m_dataMatrix2 = NetSegment.CalculateControlMatrix(position, middlePos, middlePos2, position2, data.m_position, 1f);
                    }
                } else {
                    data.m_nameData = null;
                }
                if (info.m_requireSurfaceMaps) {
                    Singleton<TerrainManager>.instance.GetSurfaceMapping(data.m_position, out data.m_dataTexture0, out data.m_dataTexture1, out data.m_dataVector1);
                } else if (info.m_requireHeightMap) {
                    Singleton<TerrainManager>.instance.GetHeightMapping(data.m_position, out data.m_dataTexture0, out data.m_dataVector1, out data.m_dataVector2);
                }
            }
            if (info.m_segments != null && (layerMask & info.m_netLayers) != 0) {
                for (int i = 0; i < info.m_segments.Length; i++) {
                    NetInfo.Segment segment = info.m_segments[i];
                    bool flag;
                    if (segment.CheckFlags(thisSegment.m_flags, out flag)) {
                        Vector4 dataVector = data.m_dataVector3;
                        Vector4 dataVector2 = data.m_dataVector0;
                        if (segment.m_requireWindSpeed) {
                            dataVector.w = data.m_dataFloat0;
                        }
                        if (flag) {
                            dataVector2.x = -dataVector2.x;
                            dataVector2.y = -dataVector2.y;
                        }
                        if (cameraInfo.CheckRenderDistance(data.m_position, segment.m_lodRenderDistance)) {
                            instance.m_materialBlock.Clear();
                            instance.m_materialBlock.SetMatrix(instance.ID_LeftMatrix, data.m_dataMatrix0);
                            instance.m_materialBlock.SetMatrix(instance.ID_RightMatrix, data.m_dataMatrix1);
                            instance.m_materialBlock.SetVector(instance.ID_MeshScale, dataVector2);
                            instance.m_materialBlock.SetVector(instance.ID_ObjectIndex, dataVector);
                            instance.m_materialBlock.SetColor(instance.ID_Color, data.m_dataColor0);
                            if (segment.m_requireSurfaceMaps && data.m_dataTexture0 != null) {
                                instance.m_materialBlock.SetTexture(instance.ID_SurfaceTexA, data.m_dataTexture0);
                                instance.m_materialBlock.SetTexture(instance.ID_SurfaceTexB, data.m_dataTexture1);
                                instance.m_materialBlock.SetVector(instance.ID_SurfaceMapping, data.m_dataVector1);
                            } else if (segment.m_requireHeightMap && data.m_dataTexture0 != null) {
                                instance.m_materialBlock.SetTexture(instance.ID_HeightMap, data.m_dataTexture0);
                                instance.m_materialBlock.SetVector(instance.ID_HeightMapping, data.m_dataVector1);
                                instance.m_materialBlock.SetVector(instance.ID_SurfaceMapping, data.m_dataVector2);
                            }
                            NetManager netManager = instance;
                            netManager.m_drawCallData.m_defaultCalls = netManager.m_drawCallData.m_defaultCalls + 1;
                            Graphics.DrawMesh(segment.m_segmentMesh, data.m_position, data.m_rotation, segment.m_segmentMaterial, segment.m_layer, null, 0, instance.m_materialBlock);
                        } else {
                            NetInfo.LodValue combinedLod = segment.m_combinedLod;
                            if (combinedLod != null) {
                                if (segment.m_requireSurfaceMaps) {
                                    if (data.m_dataTexture0 != combinedLod.m_surfaceTexA) {
                                        if (combinedLod.m_lodCount != 0) {
                                            NetSegment.RenderLod(cameraInfo, combinedLod);
                                        }
                                        combinedLod.m_surfaceTexA = data.m_dataTexture0;
                                        combinedLod.m_surfaceTexB = data.m_dataTexture1;
                                        combinedLod.m_surfaceMapping = data.m_dataVector1;
                                    }
                                } else if (segment.m_requireHeightMap && data.m_dataTexture0 != combinedLod.m_heightMap) {
                                    if (combinedLod.m_lodCount != 0) {
                                        NetSegment.RenderLod(cameraInfo, combinedLod);
                                    }
                                    combinedLod.m_heightMap = data.m_dataTexture0;
                                    combinedLod.m_heightMapping = data.m_dataVector1;
                                    combinedLod.m_surfaceMapping = data.m_dataVector2;
                                }
                                combinedLod.m_leftMatrices[combinedLod.m_lodCount] = data.m_dataMatrix0;
                                combinedLod.m_rightMatrices[combinedLod.m_lodCount] = data.m_dataMatrix1;
                                combinedLod.m_meshScales[combinedLod.m_lodCount] = dataVector2;
                                combinedLod.m_objectIndices[combinedLod.m_lodCount] = dataVector;
                                combinedLod.m_meshLocations[combinedLod.m_lodCount] = data.m_position;
                                combinedLod.m_lodMin = Vector3.Min(combinedLod.m_lodMin, data.m_position);
                                combinedLod.m_lodMax = Vector3.Max(combinedLod.m_lodMax, data.m_position);
                                if (++combinedLod.m_lodCount == combinedLod.m_leftMatrices.Length) {
                                    NetSegment.RenderLod(cameraInfo, combinedLod);
                                }
                            }
                        }
                    }
                }
            }
            if (info.m_lanes != null && ((layerMask & info.m_propLayers) != 0 || cameraInfo.CheckRenderDistance(data.m_position, info.m_maxPropDistance + 128f))) {
                bool invert2;
                NetNode.Flags startFlags;
                Color startColor;
                NetNode.Flags endFlags;
                Color endColor;
                if ((thisSegment.m_flags & NetSegment.Flags.Invert) != NetSegment.Flags.None) {
                    invert2 = true;
                    NetInfo info6 = instance.m_nodes.m_buffer[(int)thisSegment.m_endNode].Info;
                    info6.m_netAI.GetNodeState(thisSegment.m_endNode, ref instance.m_nodes.m_buffer[(int)thisSegment.m_endNode], segmentID, ref thisSegment, out startFlags, out startColor);
                    NetInfo info7 = instance.m_nodes.m_buffer[(int)thisSegment.m_startNode].Info;
                    info7.m_netAI.GetNodeState(thisSegment.m_startNode, ref instance.m_nodes.m_buffer[(int)thisSegment.m_startNode], segmentID, ref thisSegment, out endFlags, out endColor);
                } else {
                    invert2 = false;
                    NetInfo info8 = instance.m_nodes.m_buffer[(int)thisSegment.m_startNode].Info;
                    info8.m_netAI.GetNodeState(thisSegment.m_startNode, ref instance.m_nodes.m_buffer[(int)thisSegment.m_startNode], segmentID, ref thisSegment, out startFlags, out startColor);
                    NetInfo info9 = instance.m_nodes.m_buffer[(int)thisSegment.m_endNode].Info;
                    info9.m_netAI.GetNodeState(thisSegment.m_endNode, ref instance.m_nodes.m_buffer[(int)thisSegment.m_endNode], segmentID, ref thisSegment, out endFlags, out endColor);
                }
                float startAngle2 = (float)thisSegment.m_cornerAngleStart * 0.0245436933f;
                float endAngle2 = (float)thisSegment.m_cornerAngleEnd * 0.0245436933f;
                Vector4 objectIndex = new Vector4(data.m_dataVector3.x, data.m_dataVector3.y, 1f, data.m_dataFloat0);
                Vector4 objectIndex2 = new Vector4(data.m_dataVector3.z, data.m_dataVector3.w, 1f, data.m_dataFloat0);
                InfoManager.InfoMode currentMode = Singleton<InfoManager>.instance.CurrentMode;
                if (currentMode != InfoManager.InfoMode.None && !info.m_netAI.ColorizeProps(currentMode)) {
                    objectIndex.z = 0f;
                    objectIndex2.z = 0f;
                }
                int num4 = (info.m_segments != null && info.m_segments.Length != 0) ? -1 : 0;
                uint num5 = thisSegment.m_lanes;
                if ((thisSegment.m_flags & NetSegment.Flags.Collapsed) != NetSegment.Flags.None) {
                    int num6 = 0;
                    while (num6 < info.m_lanes.Length && num5 != 0u) {
                        instance.m_lanes.m_buffer[(int)((UIntPtr)num5)].RenderDestroyedInstance(cameraInfo, segmentID, num5, info, info.m_lanes[num6], startFlags, endFlags, startColor, endColor, startAngle2, endAngle2, invert2, layerMask, objectIndex, objectIndex2, ref data, ref num4);
                        num5 = instance.m_lanes.m_buffer[(int)((UIntPtr)num5)].m_nextLane;
                        num6++;
                    }
                } else {
                    int num7 = 0;
                    while (num7 < info.m_lanes.Length && num5 != 0u) {
                        instance.m_lanes.m_buffer[(int)((UIntPtr)num5)].RenderInstance(cameraInfo, segmentID, num5, info.m_lanes[num7], startFlags, endFlags, startColor, endColor, startAngle2, endAngle2, invert2, layerMask, objectIndex, objectIndex2, ref data, ref num4);
                        num5 = instance.m_lanes.m_buffer[(int)((UIntPtr)num5)].m_nextLane;
                        num7++;
                    }
                }
            }

        }

        // // private static void NetTool.RenderSegment(NetInfo info, NetSegment.Flags flags, Vector3 startPosition, Vector3 endPosition, Vector3 startDirection, Vector3 endDirection, bool smoothStart, bool smoothEnd)
        //public static void RenderSegment(
        //    NetTool netTool,
        //    NetInfo info,
        //    NetSegment.Flags flags,
        //    Vector3 startPosition,
        //    Vector3 endPosition,
        //    Vector3 startDirection,
        //    Vector3 endDirection,
        //    bool smoothStart,
        //    bool smoothEnd){}


        // //private static void RenderNode(NetInfo info, Vector3 position, Vector3 direction)
        //public static void RenderNode(NetTool netTool, NetInfo info, Vector3 position, Vector3 direction) {
        //}



    }
}
