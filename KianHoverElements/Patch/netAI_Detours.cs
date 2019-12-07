using System;
using ColossalFramework;
using ColossalFramework.Math;
using UnityEngine;
using static Kian.Mod.ShortCuts;
using static Kian.Skins.SkinManager.Util;

namespace Kian.Patch {
    public class RoadBaseAIDetours {
        public Color GetColor1(ushort segmentID, ref NetSegment data, InfoManager.InfoMode infoMode) {
            RoadBaseAI thisAI = data.Info.m_netAI as RoadBaseAI;

            switch (infoMode) {
                case InfoManager.InfoMode.Traffic:
                    return Color.Lerp(Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_targetColor, Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_negativeColor, Mathf.Clamp01((float)data.m_trafficDensity * 0.01f));
                default:
                    switch (infoMode) {
                        case InfoManager.InfoMode.None: {
                                Color color = SegColor(segmentID, thisAI.m_info.m_color);
                                if ((data.m_flags & NetSegment.Flags.Collapsed) != NetSegment.Flags.None) {
                                    float num = 0.5f;
                                    color.r = color.r * (1f - num) + num * 0.75f;
                                    color.g = color.g * (1f - num) + num * 0.75f;
                                    color.b = color.b * (1f - num) + num * 0.75f;
                                }
                                color.a = (float)(byte.MaxValue - data.m_wetness) * 0.003921569f;
                                return color;
                            }
                        default:
                            switch (infoMode) {
                                case InfoManager.InfoMode.ParkMaintenance:
                                case InfoManager.InfoMode.Post:
                                    goto IL_47F;
                            }
                            return Base.GetColor(segmentID, ref data, infoMode);
                        case InfoManager.InfoMode.CrimeRate:
                        case InfoManager.InfoMode.Health:
                            break;
                        case InfoManager.InfoMode.NoisePollution: {
                                int num2 = (int)(100 - (data.m_noiseDensity - 100) * (data.m_noiseDensity - 100) / 100);
                                int num3 = thisAI.m_noiseAccumulation * num2 / 100;
                                return CommonBuildingAI.GetNoisePollutionColor((float)num3 * 1.25f);
                            }
                        case InfoManager.InfoMode.Transport:
                            if ((thisAI.m_info.m_vehicleTypes & VehicleInfo.VehicleType.Tram) != VehicleInfo.VehicleType.None) {
                                return Color.Lerp(Singleton<InfoManager>.instance.m_properties.m_neutralColor, Singleton<TransportManager>.instance.m_properties.m_transportColors[6], 0.15f);
                            }
                            return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                    }
                    break;
                case InfoManager.InfoMode.Garbage:
                case InfoManager.InfoMode.FireSafety:
                case InfoManager.InfoMode.Education:
                case InfoManager.InfoMode.EscapeRoutes:
                    break;
                case InfoManager.InfoMode.Maintenance:
                    if ((thisAI.m_info.m_vehicleTypes & VehicleInfo.VehicleType.Car) == VehicleInfo.VehicleType.None) {
                        return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                    }
                    if (Singleton<InfoManager>.instance.CurrentSubMode == InfoManager.SubInfoMode.WaterPower) {
                        if (thisAI.m_highwayRules) {
                            return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                        }
                        return Color.Lerp(Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_negativeColor, Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_targetColor, Mathf.Clamp01((float)data.m_condition / 255f));
                    } else {
                        NetManager instance = Singleton<NetManager>.instance;
                        byte coverage = instance.m_nodes.m_buffer[(int)data.m_startNode].m_coverage;
                        byte coverage2 = instance.m_nodes.m_buffer[(int)data.m_endNode].m_coverage;
                        if (coverage == 255 || coverage2 == 255) {
                            return Singleton<CoverageManager>.instance.m_properties.m_badCoverage;
                        }
                        return Color.Lerp(Singleton<CoverageManager>.instance.m_properties.m_goodCoverage, Singleton<CoverageManager>.instance.m_properties.m_badCoverage, Mathf.Clamp01((float)(coverage + coverage2) * 0.001968504f));
                    }
                    break;
                case InfoManager.InfoMode.Snow:
                    if ((thisAI.m_info.m_vehicleTypes & VehicleInfo.VehicleType.Car) == VehicleInfo.VehicleType.None) {
                        return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                    }
                    if (Singleton<InfoManager>.instance.CurrentSubMode == InfoManager.SubInfoMode.WaterPower) {
                        if (thisAI.m_accumulateSnow) {
                            return Color.Lerp(Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_targetColor, Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_negativeColor, Mathf.Clamp01((float)data.m_wetness / 255f));
                        }
                        return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                    } else {
                        NetManager instance2 = Singleton<NetManager>.instance;
                        byte coverage3 = instance2.m_nodes.m_buffer[(int)data.m_startNode].m_coverage;
                        byte coverage4 = instance2.m_nodes.m_buffer[(int)data.m_endNode].m_coverage;
                        if (coverage3 == 255 || coverage4 == 255) {
                            return Singleton<CoverageManager>.instance.m_properties.m_badCoverage;
                        }
                        return Color.Lerp(Singleton<CoverageManager>.instance.m_properties.m_goodCoverage, Singleton<CoverageManager>.instance.m_properties.m_badCoverage, Mathf.Clamp01((float)(coverage3 + coverage4) * 0.001968504f));
                    }
                    break;
                case InfoManager.InfoMode.Destruction:
                    if ((data.m_flags & NetSegment.Flags.Collapsed) != NetSegment.Flags.None) {
                        return Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_negativeColor;
                    }
                    return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
            }
        IL_47F:
            if ((thisAI.m_info.m_vehicleTypes & VehicleInfo.VehicleType.Car) == VehicleInfo.VehicleType.None || (infoMode == InfoManager.InfoMode.CrimeRate && Singleton<InfoManager>.instance.CurrentSubMode == InfoManager.SubInfoMode.WaterPower)) {
                return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
            }
            NetManager instance3 = Singleton<NetManager>.instance;
            byte coverage5 = instance3.m_nodes.m_buffer[(int)data.m_startNode].m_coverage;
            byte coverage6 = instance3.m_nodes.m_buffer[(int)data.m_endNode].m_coverage;
            if (coverage5 == 255 || coverage6 == 255) {
                return Singleton<CoverageManager>.instance.m_properties.m_badCoverage;
            }
            return Color.Lerp(Singleton<CoverageManager>.instance.m_properties.m_goodCoverage, Singleton<CoverageManager>.instance.m_properties.m_badCoverage, Mathf.Clamp01((float)(coverage5 + coverage6) * 0.001968504f));
        }

        public Color GetColor2(ushort nodeID, ref NetNode data, InfoManager.InfoMode infoMode) {
            RoadBaseAI thisAI = data.Info.m_netAI as RoadBaseAI;

            switch (infoMode) {
                case InfoManager.InfoMode.Traffic: {
                        int num = 0;
                        NetManager instance = Singleton<NetManager>.instance;
                        int num2 = 0;
                        for (int i = 0; i < 8; i++) {
                            ushort segment = data.GetSegment(i);
                            if (segment != 0) {
                                num += (int)instance.m_segments.m_buffer[(int)segment].m_trafficDensity;
                                num2++;
                            }
                        }
                        if (num2 != 0) {
                            num /= Mathf.Min(2, num2);
                        }
                        return Color.Lerp(Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_targetColor, Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_negativeColor, Mathf.Clamp01((float)num * 0.01f));
                    }
                default:
                    switch (infoMode) {
                        case InfoManager.InfoMode.None: {
                                int num3 = 0;
                                int num4 = 0;
                                NetManager instance2 = Singleton<NetManager>.instance;
                                int num5 = 0;
                                for (int j = 0; j < 8; j++) {
                                    ushort segment2 = data.GetSegment(j);
                                    if (segment2 != 0) {
                                        if ((instance2.m_segments.m_buffer[(int)segment2].m_flags & NetSegment.Flags.Collapsed) != NetSegment.Flags.None) {
                                            num3++;
                                        }
                                        num4 += (int)instance2.m_segments.m_buffer[(int)segment2].m_wetness;
                                        num5++;
                                    }
                                }
                                if (num5 != 0) {
                                    num4 /= num5;
                                }
                                Color color = NodeColor(nodeID, thisAI.m_info.m_color);
                                if (num3 != 0) {
                                    float num6 = (float)num3 / (float)num5 * 0.5f;
                                    color.r = color.r * (1f - num6) + num6 * 0.75f;
                                    color.g = color.g * (1f - num6) + num6 * 0.75f;
                                    color.b = color.b * (1f - num6) + num6 * 0.75f;
                                }
                                color.a = (float)(255 - num4) * 0.003921569f;
                                return color;
                            }
                        default:
                            switch (infoMode) {
                                case InfoManager.InfoMode.ParkMaintenance:
                                case InfoManager.InfoMode.Post:
                                    goto IL_7FD;
                            }
                            return Base.GetColor(nodeID, ref data, infoMode);
                        case InfoManager.InfoMode.CrimeRate:
                        case InfoManager.InfoMode.Health:
                            break;
                        case InfoManager.InfoMode.NoisePollution: {
                                int num7 = 0;
                                if (thisAI.m_noiseAccumulation != 0) {
                                    NetManager instance3 = Singleton<NetManager>.instance;
                                    int num8 = 0;
                                    for (int k = 0; k < 8; k++) {
                                        ushort segment3 = data.GetSegment(k);
                                        if (segment3 != 0) {
                                            num7 += (int)instance3.m_segments.m_buffer[(int)segment3].m_noiseDensity;
                                            num8++;
                                        }
                                    }
                                    if (num8 != 0) {
                                        num7 /= num8;
                                    }
                                }
                                int num9 = 100 - (num7 - 100) * (num7 - 100) / 100;
                                int num10 = thisAI.m_noiseAccumulation * num9 / 100;
                                return CommonBuildingAI.GetNoisePollutionColor((float)num10 * 1.25f);
                            }
                        case InfoManager.InfoMode.Transport: {
                                NetManager instance4 = Singleton<NetManager>.instance;
                                bool flag = false;
                                for (int l = 0; l < 8; l++) {
                                    ushort segment4 = data.GetSegment(l);
                                    if (segment4 != 0) {
                                        NetInfo info = instance4.m_segments.m_buffer[(int)segment4].Info;
                                        if (info != null && (info.m_vehicleTypes & VehicleInfo.VehicleType.Tram) != VehicleInfo.VehicleType.None) {
                                            flag = true;
                                        }
                                    }
                                }
                                if (flag) {
                                    return Color.Lerp(Singleton<InfoManager>.instance.m_properties.m_neutralColor, Singleton<TransportManager>.instance.m_properties.m_transportColors[6], 0.15f);
                                }
                                return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                            }
                    }
                    break;
                case InfoManager.InfoMode.Garbage:
                case InfoManager.InfoMode.FireSafety:
                case InfoManager.InfoMode.Education:
                case InfoManager.InfoMode.EscapeRoutes:
                    break;
                case InfoManager.InfoMode.Maintenance:
                    if (Singleton<InfoManager>.instance.CurrentSubMode == InfoManager.SubInfoMode.WaterPower) {
                        int num11 = 0;
                        NetManager instance5 = Singleton<NetManager>.instance;
                        int num12 = 0;
                        bool flag2 = false;
                        bool flag3 = true;
                        for (int m = 0; m < 8; m++) {
                            ushort segment5 = data.GetSegment(m);
                            if (segment5 != 0) {
                                num11 += (int)instance5.m_segments.m_buffer[(int)segment5].m_condition;
                                num12++;
                                NetInfo info2 = instance5.m_segments.m_buffer[(int)segment5].Info;
                                if (info2 != null) {
                                    if ((info2.m_vehicleTypes & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None) {
                                        flag2 = true;
                                    }
                                    RoadBaseAI roadBaseAI = info2.m_netAI as RoadBaseAI;
                                    if (roadBaseAI == null || !roadBaseAI.m_highwayRules) {
                                        flag3 = false;
                                    }
                                }
                            }
                        }
                        if (num12 != 0) {
                            num11 /= num12;
                        }
                        if (flag2 && !flag3) {
                            return Color.Lerp(Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_negativeColor, Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_targetColor, Mathf.Clamp01((float)num11 / 255f));
                        }
                        return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                    } else {
                        NetManager instance6 = Singleton<NetManager>.instance;
                        bool flag4 = false;
                        for (int n = 0; n < 8; n++) {
                            ushort segment6 = data.GetSegment(n);
                            if (segment6 != 0) {
                                NetInfo info3 = instance6.m_segments.m_buffer[(int)segment6].Info;
                                if (info3 != null && (info3.m_vehicleTypes & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None) {
                                    flag4 = true;
                                }
                            }
                        }
                        if (flag4) {
                            return Color.Lerp(Singleton<CoverageManager>.instance.m_properties.m_goodCoverage, Singleton<CoverageManager>.instance.m_properties.m_badCoverage, Mathf.Clamp01((float)data.m_coverage * 0.003937008f));
                        }
                        return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                    }
                    break;
                case InfoManager.InfoMode.Snow:
                    if (Singleton<InfoManager>.instance.CurrentSubMode == InfoManager.SubInfoMode.WaterPower) {
                        int num13 = 0;
                        NetManager instance7 = Singleton<NetManager>.instance;
                        int num14 = 0;
                        bool flag5 = false;
                        bool flag6 = false;
                        for (int num15 = 0; num15 < 8; num15++) {
                            ushort segment7 = data.GetSegment(num15);
                            if (segment7 != 0) {
                                num13 += (int)instance7.m_segments.m_buffer[(int)segment7].m_wetness;
                                num14++;
                                NetInfo info4 = instance7.m_segments.m_buffer[(int)segment7].Info;
                                if (info4 != null) {
                                    if ((info4.m_vehicleTypes & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None) {
                                        flag5 = true;
                                    }
                                    RoadBaseAI roadBaseAI2 = info4.m_netAI as RoadBaseAI;
                                    if (roadBaseAI2 != null && roadBaseAI2.m_accumulateSnow) {
                                        flag6 = true;
                                    }
                                }
                            }
                        }
                        if (num14 != 0) {
                            num13 /= num14;
                        }
                        if (flag5 && flag6) {
                            return Color.Lerp(Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_targetColor, Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_negativeColor, Mathf.Clamp01((float)num13 / 255f));
                        }
                        return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                    } else {
                        NetManager instance8 = Singleton<NetManager>.instance;
                        bool flag7 = false;
                        for (int num16 = 0; num16 < 8; num16++) {
                            ushort segment8 = data.GetSegment(num16);
                            if (segment8 != 0) {
                                NetInfo info5 = instance8.m_segments.m_buffer[(int)segment8].Info;
                                if (info5 != null && (info5.m_vehicleTypes & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None) {
                                    flag7 = true;
                                }
                            }
                        }
                        if (flag7) {
                            return Color.Lerp(Singleton<CoverageManager>.instance.m_properties.m_goodCoverage, Singleton<CoverageManager>.instance.m_properties.m_badCoverage, Mathf.Clamp01((float)data.m_coverage * 0.003937008f));
                        }
                        return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                    }
                    break;
                case InfoManager.InfoMode.Destruction: {
                        NetManager instance9 = Singleton<NetManager>.instance;
                        bool flag8 = false;
                        for (int num17 = 0; num17 < 8; num17++) {
                            ushort segment9 = data.GetSegment(num17);
                            if (segment9 != 0 && (instance9.m_segments.m_buffer[(int)segment9].m_flags & NetSegment.Flags.Collapsed) != NetSegment.Flags.None) {
                                flag8 = true;
                            }
                        }
                        if (flag8) {
                            return Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_negativeColor;
                        }
                        return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                    }
            }
        IL_7FD:
            NetManager instance10 = Singleton<NetManager>.instance;
            bool flag9 = false;
            for (int num18 = 0; num18 < 8; num18++) {
                ushort segment10 = data.GetSegment(num18);
                if (segment10 != 0) {
                    NetInfo info6 = instance10.m_segments.m_buffer[(int)segment10].Info;
                    if (info6 != null && (info6.m_vehicleTypes & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None) {
                        flag9 = true;
                    }
                }
            }
            if (!flag9 || (infoMode == InfoManager.InfoMode.CrimeRate && Singleton<InfoManager>.instance.CurrentSubMode == InfoManager.SubInfoMode.WaterPower)) {
                return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
            }
            return Color.Lerp(Singleton<CoverageManager>.instance.m_properties.m_goodCoverage, Singleton<CoverageManager>.instance.m_properties.m_badCoverage, Mathf.Clamp01((float)data.m_coverage * 0.003937008f));
        }


        public static class Base {
            public static Color GetColor(ushort nodeID, ref NetNode data, InfoManager.InfoMode infoMode) {
                NetAI thisAI = data.Info.m_netAI;

                if (infoMode != InfoManager.InfoMode.None) {
                    return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                }
                return NodeColor(nodeID, thisAI.m_info.m_color);
            }

            public static Color GetColor(ushort segmentID, ref NetSegment data, InfoManager.InfoMode infoMode) {
                NetAI thisAI = data.Info.m_netAI;

                if (infoMode != InfoManager.InfoMode.None) {
                    return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                }
                return SegColor(segmentID, thisAI.m_info.m_color);
            }

        }// end class Base
    } // end class RoadBaseAI detour
} //end name space