using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Transport;
using UnityEngine;

public unsafe class NativeTests : MonoBehaviour {
  [StructLayout(LayoutKind.Explicit)]
  struct TransformData {
    public const int SIZE      = 28;
    public const int ALIGNMENT = 4;

    [FieldOffset(0)]
    public Vector3 Positon;

    [FieldOffset(12)]
    public Quaternion Rotation;
  }

  [StructLayout(LayoutKind.Explicit)]
  struct CharacterData {
    public const int SIZE      = 12;
    public const int ALIGNMENT = 4;

    [FieldOffset(0)]
    public float Health;

    [FieldOffset(4)]
    public float Energy;

    [FieldOffset(8)]
    public int Level;
  }

  [StructLayout(LayoutKind.Explicit)]
  struct EquipmentData {
    public const int SIZE      = 8;
    public const int ALIGNMENT = 4;

    [FieldOffset(0)]
    public int Helmet;

    [FieldOffset(4)]
    public int Shoulders;
  }

  enum Types {
    Npc = 1,
  }

  [StructLayout(LayoutKind.Explicit)]
  struct WorldObject {
    public const int SIZE      = 28;
    public const int ALIGNMENT = 4;

    [FieldOffset(0)]
    public Types Type;

    [FieldOffset(4)]
    public TransformData Transform;
  }

  [StructLayout(LayoutKind.Explicit)]
  struct Npc {
    public const int SIZE      = 48;
    public const int ALIGNMENT = 4;

    [FieldOffset(0)]
    public Types Type;

    [FieldOffset(4)]
    public TransformData Transform;

    [FieldOffset(4 + TransformData.SIZE)]
    public CharacterData Character;

    [FieldOffset(4 + TransformData.SIZE + CharacterData.SIZE)]
    public EquipmentData Equipment;
  }

  struct ObjHeader {
    public ObjHeader* Prev;
    public ObjHeader* Next;
  }

// struct PlayerData {
//   public fixed char Name[16];
// }
//

//
// struct Player {
//   public PlayerData    Players;
//   public TransformData Transform;
//   public CharacterData Character;
//   public EquipmentData Equipment;
// }
//


//
// // struct ColorWithoutAlpha {
// //   public byte R;
// //   public byte G;
// //   public byte B;
// // }
//
//

  [StructLayout(LayoutKind.Explicit)]
  struct NotifyHeader {
    public const int ACTUAL_SIZE  = 16;
    public const int NETWORK_SIZE = 13;

    [FieldOffset(0)]
    public byte PacketType;

    [FieldOffset(1)]
    public ushort SequenceForPacket;

    [FieldOffset(3)]
    public ushort SequenceForAcks;

    [FieldOffset(5)]
    public ulong MaskForAcks;
  }

  // [StructLayout(LayoutKind.Explicit)]
  // struct MyMessage {
  //   [FieldOffset(0)]
  //   public int Foo;
  //
  //   [FieldOffset(4)]
  //   public int Bar;
  //
  //   [FieldOffset(5)]
  //   public ulong Data;
  // }

//
//
// struct MessageHeader {
//   
// }

  void PrintPosition(CharacterData* cd) {
    // CharacterData empty = default;
    // *cd = empty;
    // Debug.Log(wo->Transform.Positon);
    // Debug.Log(wo->Type);
  }

  void Start() {
    // NotifyHeader header = new NotifyHeader();
    // header.PacketType        = 4;
    // header.MaskForAcks       = 123123129;
    // header.SequenceForAcks   = 4;
    // header.SequenceForPacket = 81;
    //
    // void* packetData = null;
    //
    // Native.MemCpy(packetData, &header, NotifyHeader.NETWORK_SIZE);

    //var wordAlignedSize = Native.RoundToAlignment(sizeof(ColorWithoutAlpha), 8);
    //Debug.Log(sizeof(ColorWithoutAlpha));
    // SIMD = Single Instruction Multiple Data
    //
    // Native.MallocAndClearBlock(sizeof(ObjHeader), sizeof(Npc), out var npcHeaderPtr, out var npcPtr);
    //
    // var npc = (Npc*) npcPtr;
    // npc->Type              = Types.Npc;
    // npc->Transform.Positon = Vector3.forward;
    //
    // PrintPosition((WorldObject*) npc);
    //

    //
    // Native.MallocAndClearBlock(sizeof(ObjHeader), sizeof(Player), out var playerHeaderPtr, out var player);
    // var playerHeader = (ObjHeader*) playerHeaderPtr;
    //
    // npcHeader->Next    = playerHeader;
    // playerHeader->Prev = npcHeader;
    //
    // var current = npcHeader;
    //
    // while (current != null) {
    //   Debug.Log(new IntPtr(current));
    //   current = current->Next;
    // }

    // var transform = (TransformData*) t;
    // var character = (CharacterData*) c;

    // var transformSize = Native.RoundToAlignment(sizeof(TransformData), 8);
    //
    // Debug.Log(transformSize);
    //
    // var ptr = (byte*)Native.MallocAndClear(transformSize + sizeof(CharacterData));
    //  
    // //
    //
    // var transform = (TransformData*) (ptr + 0);
    // var character = (CharacterData*) (ptr + transformSize);
    //
    //
    //var transform = Native.MallocAndClear<TransformData>();
    //var character = Native.MallocAndClear<CharacterData>();
  }
}