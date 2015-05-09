﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellEditor.Sources.DBC
{
    class SpellRadius
    {
        // Begin Window
        private MainWindow main;
        private SpellDBC spell;
        // End Window

        // Begin DBCs
        public DBC_Header header;
        public SpellRadiusMap body;
        // End DBCs

        public SpellRadius(MainWindow window, SpellDBC spellDBC)
        {
            main = window;
            spell = spellDBC;

            for (UInt32 i = 0; i < header.RecordCount; ++i)
            {
                body.records[i].ID = new UInt32();
                body.records[i].Radius = new float();
                body.records[i].RadiusPerLevel = new float();
                body.records[i].MaximumRadius = new float();
            }

            if (!File.Exists("DBC/SpellRadius.dbc"))
            {
                main.HandleErrorMessage("SpellRadius.dbc was not found!");

                return;
            }

            FileStream fileStream = new FileStream("DBC/SpellRadius.dbc", FileMode.Open);
            int count = Marshal.SizeOf(typeof(DBC_Header));
            byte[] readBuffer = new byte[count];
            BinaryReader reader = new BinaryReader(fileStream);
            readBuffer = reader.ReadBytes(count);
            GCHandle handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
            header = (DBC_Header)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DBC_Header));
            handle.Free();

            body.records = new SpellRadiusRecord[header.RecordCount];

            for (UInt32 i = 0; i < header.RecordCount; ++i)
            {
                count = Marshal.SizeOf(typeof(SpellRadiusRecord));
                readBuffer = new byte[count];
                reader = new BinaryReader(fileStream);
                readBuffer = reader.ReadBytes(count);
                handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
                body.records[i] = (SpellRadiusRecord)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(SpellRadiusRecord));
                handle.Free();
            }

            reader.Close();
            fileStream.Close();

            body.lookup = new List<RadiusLookup>();

            int boxIndex = 1;

            main.RadiusIndex1.Items.Add("0 - 0");
            main.RadiusIndex2.Items.Add("0 - 0");
            main.RadiusIndex3.Items.Add("0 - 0");

            RadiusLookup t;

            t.ID = 0;
            t.comboBoxIndex = 0;

            body.lookup.Add(t);

            for (UInt32 i = 0; i < header.RecordCount; ++i)
            {
                int radius = (int)body.records[i].Radius;
                int maximumRadius = (int)body.records[i].MaximumRadius;

                RadiusLookup temp;

                temp.ID = (int)body.records[i].ID;
                temp.comboBoxIndex = boxIndex;

                main.RadiusIndex1.Items.Add(radius + " - " + maximumRadius);
                main.RadiusIndex2.Items.Add(radius + " - " + maximumRadius);
                main.RadiusIndex3.Items.Add(radius + " - " + maximumRadius);

                body.lookup.Add(temp);

                boxIndex++;
            }
        }

        public void UpdateRadiusIndexes()
        {
            int[] IDs = { (int)spell.body.records[main.selectedID].record.EffectRadiusIndex1, (int)spell.body.records[main.selectedID].record.EffectRadiusIndex2, (int)spell.body.records[main.selectedID].record.EffectRadiusIndex3 };

            for (int j = 0; j < IDs.Length; ++j)
            {
                int ID = IDs[j];

                if (ID == 0)
                {
                    switch (j)
                    {
                        case 0:
                            {
                                main.RadiusIndex1.SelectedIndex = 0;

                                break;
                            }

                        case 1:
                            {
                                main.RadiusIndex2.SelectedIndex = 0;

                                break;
                            }

                        case 2:
                            {
                                main.RadiusIndex3.SelectedIndex = 0;

                                break;
                            }

                        default: { break; }
                    }

                    continue;
                }

                for (int i = 0; i < body.lookup.Count; ++i)
                {
                    if (ID == body.lookup[i].ID)
                    {
                        switch (j)
                        {
                            case 0:
                                {
                                    main.RadiusIndex1.SelectedIndex = body.lookup[i].comboBoxIndex;

                                    break;
                                }

                            case 1:
                                {
                                    main.RadiusIndex2.SelectedIndex = body.lookup[i].comboBoxIndex;

                                    break;
                                }

                            case 2:
                                {
                                    main.RadiusIndex3.SelectedIndex = body.lookup[i].comboBoxIndex;

                                    break;
                                }

                            default: { break; }
                        }

                        continue;
                    }
                }
            }
        }

        public struct SpellRadiusMap
        {
            public SpellRadiusRecord[] records;
            public List<RadiusLookup> lookup;
        };

        public struct RadiusLookup
        {
            public int ID;
            public int comboBoxIndex;
        };

        public struct SpellRadiusRecord
        {
            public UInt32 ID;
            public float Radius;
            public float RadiusPerLevel;
            public float MaximumRadius;
        };
    };
}
