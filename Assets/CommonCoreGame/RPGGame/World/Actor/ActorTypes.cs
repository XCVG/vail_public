﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonCore.World;
using CommonCore.RpgGame.Rpg;

namespace CommonCore.RpgGame.World
{
    public enum ActorInteractionType
    {
        None, Special, AmbientMonologue, Dialogue, Script
    }

    public enum ActorAnimState
    {
        Idle, Dead, Dying, Walking, Running, Hurting, Talking, Shooting, Punching, Pickup
    }

    public enum ActorAiState
    {
        Idle, Dead, Wandering, Chasing, Hurting, Attacking, Covering, Fleeing, ScriptedMoveTo, ScriptedHalt
    }

    public enum ActorBodyPart
    {
        Unspecified, Torso, Head, LeftArm, LeftLeg, RightArm, RightLeg, Tail
    }

    //this is in flux, we may change what data we store in the future
    public class ActorExtraData
    {
        //state information (very important, so we save most of it)
        public ActorAiState CurrentAiState { get; set; }
        public ActorAiState LastAiState { get; set; }
        public bool LockAiState { get; set; }
        public ActorAnimState CurrentAnimState { get; set; }
        public bool LockAnimState { get; set; }
        public string SavedTarget { get; set; }
        public Vector3 AltTarget { get; set; }
        public float TimeInState { get; set; }

        //health 
        public float Health { get; set; }
        public bool BeenHit { get; set; }

        //navigation
        public bool IsRunning { get; set; }

        //interaction
        public bool InteractionForceDisabled { get; set; }

        public ActorExtraData()
        {

        }
    }
}