using System;
using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum CargoTransferDirection
    {
        ToShip,
        ToSRV,
        ToCarrier,
        UnrecognisedValue
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum CarrierCrewOperation
    {
        Activate,
        Deactivate,
        Pause,
        Resume,
        Replace,
        UnrecognisedValue
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum CarrierDockingAccess
    {
        All,
        None,
        Friends,
        Squadron,
        SquadronFriends,
        UnrecognisedValue
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum CarrierOperation
    {
        BuyPack,
        SellPack,
        RestockPack,
        UnrecognisedValue
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum ContributionType
    {
        Commodity,
        Materials,
        Credits,
        Bond,
        Bounty,
        UnrecognisedValue
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum CrimeType
    {
        Assault,
        Murder,
        Piracy,
        Interdiction,
        IllegalCargo,
        DisobeyPolice,
        FireInNoFireZone,
        FireInStation,
        DumpingDangerous,
        DumpingNearStation,
        DockingMinorBlockingAirlock,
        DockingMajorBlockingAirlock,
        DockingMinorBlockingLandingPad,
        DockingMajorBlockingLandingPad,
        DockingMinorTresspass,
        DockingMajorTresspass,
        CollidedAtSpeedInNoFireZone,
        CollidedAtSpeedInNoFireZone_HullDamage,
        RecklessWeaponsDischarge,
        PassengerWanted,
        onfoot_carryingIllegalGoods,
        onfoot_identityTheft,
        onfoot_recklessEndangerment,
        onfoot_arcCutterUse,
        onfoot_detectionOfWeapon,
        onfoot_murder,
        onfoot_overchargeIntent,
        onfoot_damagingDefences,
        onfoot_overchargedPort,
        onfoot_trespass,
        onfoot_carryingIllegalData,
        onfoot_propertyTheft,
        onfoot_theft,
        onfoot_profileCloningIntent,
        onFoot_failureToSubmitToPolice,
        onfoot_dataTransfer,
        onFoot_carryingStolenGoods,
        onFoot_eBreachUse,
        onFoot_breakingAndEntering,
        shuttleDestruction,
        collidedWithDamage,
        UnrecognisedValue
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum LimpetDrone
    {
        Hatchbreaker,
        FuelTransfer,
        Collection,
        Prospector,
        Repair,
        Research,
        Decontamination,
        Recon,
        UnrecognisedValue
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum FriendStatus
    {
        Requested,
        Declined,
        Added,
        Lost,
        Offline,
        Online,
        UnrecognisedValue
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: Unknown)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum MicroCategory
    {
        Encoded,
        Raw,
        Manufactured,
        Item,
        Component,
        Data,
        Consumable,
        Unknown
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum MicroTransferDirection
    {
        ToBackpack,
        ToShipLocker,
        UnrecognisedValue
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum MissionEffect
    {
        None,
        Low,
        Med,
        High,
        UnrecognisedValue
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum ParentType
    {
        Null,
        Planet,
        Star,
        UnrecognisedValue
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum Progress
    {
        Invited,
        Acquainted,
        Unlocked,
        Barred,
        Known,
        UnrecognisedValue
    }

    public enum RankCombat
    {
        Harmless = 0,
        MostlyHarmless = 1,
        Novice = 2,
        Competent = 3,
        Expert = 4,
        Master = 5,
        Dangerous = 6,
        Deadly = 7,
        Elite = 8,
        Elite1 = 9,
        Elite2 = 10,
        Elite3 = 11,
        Elite4 = 12,
        Elite5 = 13
    }

    public enum RankTrade
    {
        Penniless = 0,
        MostlyPenniless = 1,
        Peddler = 2,
        Dealer = 3,
        Merchant = 4,
        Broker = 5,
        Entrepreneur = 6,
        Tycoon = 7,
        Elite = 8,
        Elite1 = 9,
        Elite2 = 10,
        Elite3 = 11,
        Elite4 = 12,
        Elite5 = 13
    }

    public enum RankExploration
    {
        Aimless = 0,
        MostlyAimless = 1,
        Scout = 2,
        Surveyor = 3,
        Explorer = 4,
        Pathfinder = 5,
        Ranger = 6,
        Pioneer = 7,
        Elite = 8,
        Elite1 = 9,
        Elite2 = 10,
        Elite3 = 11,
        Elite4 = 12,
        Elite5 = 13
    }

    public enum RankCQC
    {
        Helpless = 0,
        MostlyHelpless = 1,
        Amateur = 2,
        SemiProfessional = 3,
        Professional = 4,
        Champion = 5,
        Hero = 6,
        Legend = 7,
        Elite = 8,
        Elite1 = 9,
        Elite2 = 10,
        Elite3 = 11,
        Elite4 = 12,
        Elite5 = 13
    }

    public enum RankExobiologist
    {
        Directionless = 0,
        MostlyDirectionless = 1,
        Compiler = 2,
        Collector = 3,
        Cataloguer = 4,
        Taxonomist = 5,
        Ecologist = 6,
        Geneticist = 7,
        Elite = 8,
        Elite1 = 9,
        Elite2 = 10,
        Elite3 = 11,
        Elite4 = 12,
        Elite5 = 13
    }

    public enum RankSoldier
    {
        Defenceless = 0,
        MostlyDefenceless = 1,
        Rookie = 2,
        Soldier = 3,
        Gunslinger = 4,
        Warrior = 5,
        Gladiator = 6,
        Deadeye = 7,
        Elite = 8,
        Elite1 = 9,
        Elite2 = 10,
        Elite3 = 11,
        Elite4 = 12,
        Elite5 = 13
    }

    public enum RankFederation
    {
        None = 0,
        Recruit = 1,
        Cadet = 2,
        Midshipman = 3,
        PettyOfficer = 4,
        ChiefPettyOfficer = 5,
        WarrantOfficer = 6,
        Ensign = 7,
        Lieutenant = 8,
        LtCommander = 9,
        PostCommander = 10,
        PostCaptain = 11,
        RearAdmiral = 12,
        ViceAdmiral = 13,
        Admiral = 14
    }

    public enum RankEmpire
    {
        None = 0,
        Outsider = 1,
        Serf = 2,
        Master = 3,
        Squire = 4,
        Knight = 5,
        Lord = 6,
        Baron = 7,
        Viscount = 8,
        Count = 9,
        Earl = 10,
        Marquis = 11,
        Duke = 12,
        Prince = 13,
        King = 14
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum Reason
    {
        NoSpace,
        TooLarge,
        Hostile,
        Offences,
        Distance,
        ActiveFighter,
        JumpImminent,
        RestrictedAccess,
        NoReason,
        DockOffline,
        UnrecognisedValue
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum ScanOrganicType
    {
        Log,
        Sample, 
        Analyse,
        UnrecognisedValue
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum ScanType
    {
        Cargo,
        Crime,
        Cabin,
        Data,
        Unknown,
        Xeno,
        UnrecognisedValue
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: None)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    [System.Flags]
    public enum StationService : ulong
    {
        None = 0,
        Dock = 1,
        Autodock = 1 << 1,
        BlackMarket = 1 << 2,
        Commodities = 1 << 3,
        Contacts = 1 << 4,
        Exploration = 1 << 5,
        Initiatives = 1 << 6,
        Missions = 1 << 7,
        Outfitting = 1 << 8,
        CrewLounge = 1 << 9,
        Rearm = 1 << 10,
        Refuel = 1 << 11,
        Repair = 1 << 12,
        Shipyard = 1 << 13,
        Tuning = 1 << 14,
        Workshop = 1 << 15,
        MissionsGenerated = 1 << 16,
        Facilitator = 1 << 17,
        Research = 1 << 18,
        FlightController = 1 << 19,
        StationOperations = 1 << 20,
        OnDockMission = 1 << 21,
        Powerplay = 1 << 22,
        SearchAndRescue = 1 << 23,
        SearchRescue = 1 << 23,
        MaterialTrader = 1 << 24,
        TechBroker = 1 << 25,
        StationMenu = 1 << 26,
        Shop = 1 << 27,
        Engineer = 1 << 28,
        CarrierManagement = 1 << 29,
        CarrierFuel = 1 << 30,
        VoucherRedemption = 1L << 31,
        CarrierVendor = 1L << 32,
        ModulePacks = 1L << 33,
        Livery = 1L << 34,
        SocialSpace = 1L << 35,
        Bartender = 1L << 36,
        VistaGenomics = 1L << 37,
        PioneerSupplies = 1L << 38, 
        ApexInterstellar = 1L << 39,
        FrontlineSolutions = 1L << 40,
        RegisteringColonisation = 1L << 41,
        ColonisationContribution = 1L << 42,
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum TextChannel
    {
        Wing,
        Local,
        Voicechat,
        Friend,
        Player,
        Npc,
        Squadron,
        SquadLeaders,
        Starsystem,
        UnrecognisedValue
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum TrendValue
    {
        UpGood,
        UpBad,
        DownGood,
        DownBad,
        None,
        UnrecognisedValue
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum UpdateType
    {
        Collect,
        Deliver,
        WingUpdate,
        UnrecognisedValue
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum VehicleSwitchTo
    {
        Mothership,
        Fighter,
        SRV,
        UnrecognisedValue
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum VoucherType
    {
        CombatBond,
        Bounty,
        Trade,
        Settlement,
        Scannable,
        Codex,
        None,
        UnrecognisedValue
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum CarrierType
    {
        FleetCarrier,
        SquadronCarrier,
        UnrecognisedValue
    }

    [Flags]
    public enum StatusFlags : uint
    {
        Docked = 1,
        Landed = 1 << 1,
        LandingGear = 1 << 2,
        Shields = 1 << 3,
        Supercruise = 1 << 4,
        FAOff = 1 << 5,
        Hardpoints = 1 << 6,
        Wing = 1 << 7,
        Lights = 1 << 8,
        CargoScoop = 1 << 9,
        SilentRunning = 1 << 10,
        FuelScooping = 1 << 11,
        SRVBrake = 1 << 12,
        SRVTurret = 1 << 13,
        SRVProximity = 1 << 14,
        SRVDriveAssist = 1 << 15,
        Masslock = 1 << 16,
        FSDCharging = 1 << 17,
        FSDCooldown = 1 << 18,
        LowFuel = 1 << 19,
        Overheat = 1 << 20,
        LatLongValid = 1 << 21,
        InDanger = 1 << 22,
        Interdiction = 1 << 23,
        MainShip = 1 << 24,
        Fighter = 1 << 25,
        SRV = 1 << 26,
        AnalysisHUD = 1 << 27,
        NightVision = 1 << 28,
        /// <summary>
        /// Altitude above average radius (sea level) when set. Altitude raycast to ground when unset.
        /// </summary>
        RadialAltitude = 1 << 29,
        FSDJump = 1 << 30,
        SRVHighBeam = 1u << 31
    }

    [Flags]
    public enum StatusFlags2 : uint
    {
        OnFoot = 1,
        InTaxi = 1 << 1,
        InMulticrew = 1 << 2,
        OnFootInStation = 1 << 3,
        OnFootOnPlanet = 1 << 4,
        AimDownSight = 1 << 5,
        LowOxygen = 1 << 6,
        LowHealth = 1 << 7,
        Cold = 1 << 8,
        Hot = 1 << 9,
        VeryCold = 1 << 10,
        VeryHot = 1 << 11,
        GlideMode = 1 << 12,
        OnFootInHangar = 1 << 13,
        OnFootInSocialSpace = 1 << 14,
        OnFootExterior = 1 << 15,
        BreathableAtmosphere = 1 << 16,
        TelepresenceMulticrew = 1 << 17,
        PhysicalMulticrew = 1 << 18,
        FsdHyperdriveCharging = 1 << 19
    }

    [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: UnrecognisedValue)]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum LegalStatus
    {
        Clean,
        IllegalCargo,
        Speeding,
        Wanted,
        Hostile,
        PassengerWanted,
        Warrant,
        Thargoid,
        Allied,
        Lawless,
        Enemy,
        Unknown,
        Hunter,
        UnrecognisedValue
    }

    public enum FocusStatus
    {
        NoFocus = 0,
        InternalPanel = 1,
        ExternalPanel = 2,
        CommsPanel = 3,
        RolePanel = 4,
        StationServices = 5,
        GalaxyMap = 6,
        SystemMap = 7,
        Orrery = 8,
        FSS = 9,
        SAA = 10,
        Codex = 11
    }
}
