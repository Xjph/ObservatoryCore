using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Passengers
    {
        [JsonPropertyName("Passengers_Missions_Accepted")]
        public int MissionsAccepted { get; init; }

        [JsonPropertyName("Passengers_Missions_Disgruntled")]
        public int MissionsDisgruntled { get; init; }

        [JsonPropertyName("Passengers_Missions_Bulk")]
        public int MissionsBulk { get; init; }

        [JsonPropertyName("Passengers_Missions_VIP")]
        public int MissionsVIP { get; init; }

        [JsonPropertyName("Passnegers_Missions_Delivered")]
        public int MissionsDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Ejected")]
        public int MissionsEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Tourist_vip_delivered")]
        public int MissionsTouristVIPDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Tourist_vip_ejected")]
        public int MissionsTouristVIPEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Tourist_bulk_delivered")]
        public int MissionsTouristBulkDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Tourist_bulk_ejected")]
        public int MissionsTouristBulkEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Refugee_vip_delivered")]
        public int MissionsRefugeeVIPDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Refugee_vip_ejected")]
        public int MissionsRefugeeVIPEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Refugee_bulk_delivered")]
        public int MissionsRefugeeBulkDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Refugee_bulk_ejected")]
        public int MissionsRefugeeBulkEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_AidWorker_vip_delivered")]
        public int MissionsAidWorkerVIPDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_AidWorker_vip_ejected")]
        public int MissionsAidWorkerVIPEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_AidWorker_bulk_delivered")]
        public int MissionsAidWorkerBulkDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_AidWorker_bulk_ejected")]
        public int MissionsAidWorkerBulkEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_POW_vip_delivered")]
        public int MissionsPOWVIPDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_POW_vip_ejected")]
        public int MissionsPOWVIPEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_POW_bulk_delivered")]
        public int MissionsPOWBulkDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_POW_bulk_ejected")]
        public int MissionsPOWBulkEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Soldiers_vip_delivered")]
        public int MissionsSoldiersVIPDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Soldiers_vip_ejected")]
        public int MissionsSoldiersVIPEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Soldiers_bulk_delivered")]
        public int MissionsSoldiersBulkDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Soldiers_bulk_ejected")]
        public int MissionsSoldiersBulkEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Protestors_vip_delivered")]
        public int MissionsProtestorsVIPDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Protestors_vip_ejected")]
        public int MissionsProtestorsVIPEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Protestors_bulk_delivered")]
        public int MissionsProtestorsBulkDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Protestors_bulk_ejected")]
        public int MissionsProtestorsBulkEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Prisoners_vip_delivered")]
        public int MissionsPrisonersVIPDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Prisoners_vip_ejected")]
        public int MissionsPrisonersVIPEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Prisoners_bulk_delivered")]
        public int MissionsPrisonersBulkDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Prisoners_bulk_ejected")]
        public int MissionsPrisonersBulkEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_FreedomFighters_vip_delivered")]
        public int MissionsFreedomFightersVIPDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_FreedomFighters_vip_ejected")]
        public int MissionsFreedomFightersVIPEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_FreedomFighters_bulk_delivered")]
        public int MissionsFreedomFightersBulkDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_FreedomFighters_bulk_ejected")]
        public int MissionsFreedomFightersBulkEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Politicians_vip_delivered")]
        public int MissionsPoliticiansVIPDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Politicians_vip_ejected")]
        public int MissionsPoliticiansVIPEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Politicians_bulk_delivered")]
        public int MissionsPoliticiansBulkDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Politicians_bulk_ejected")]
        public int MissionsPoliticiansBulkEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_PoliticalPrisoner_vip_delivered")]
        public int MissionsPoliticalPrisonerVIPDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_PoliticalPrisoner_vip_ejected")]
        public int MissionsPoliticalPrisonerVIPEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_PoliticalPrisoner_bulk_delivered")]
        public int MissionsPoliticalPrisonerBulkDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_PoliticalPrisoner_bulk_ejected")]
        public int MissionsPoliticalPrisonerBulkEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_SecurityForces_vip_delivered")]
        public int MissionsSecurityForcesVIPDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_SecurityForces_vip_ejected")]
        public int MissionsSecurityForcesVIPEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_SecurityForces_bulk_delivered")]
        public int MissionsSecurityForcesBulkDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_SecurityForces_bulk_ejected")]
        public int MissionsSecurityForcesBulkEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Businessmen_vip_delivered")]
        public int MissionsBusinessmenVIPDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Businessmen_vip_ejected")]
        public int MissionsBusinessmenVIPEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Businessmen_bulk_delivered")]
        public int MissionsBusinessmenBulkDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Businessmen_bulk_ejected")]
        public int MissionsBusinessmenBulkEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_ScienceTeams_vip_delivered")]
        public int MissionsScienceTeamsVIPDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_ScienceTeams_vip_ejected")]
        public int MissionsScienceTeamsVIPEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_ScienceTeams_bulk_delivered")]
        public int MissionsScienceTeamsBulkDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_ScienceTeams_bulk_ejected")]
        public int MissionsScienceTeamsBulkEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Explorers_vip_delivered")]
        public int MissionsExplorersVIPDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Explorers_vip_ejected")]
        public int MissionsExplorersVIPEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Explorers_bulk_delivered")]
        public int MissionsExplorersBulkDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Explorers_bulk_ejected")]
        public int MissionsExplorersBulkEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Celebrities_vip_delivered")]
        public int MissionsCelebritiesVIPDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Celebrities_vip_ejected")]
        public int MissionsCelebritiesVIPEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Celebrities_bulk_delivered")]
        public int MissionsCelebritiesBulkDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Celebrities_bulk_ejected")]
        public int MissionsCelebritiesBulkEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_HeadOfState_vip_delivered")]
        public int MissionsHeadOfStateVIPDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_HeadOfState_vip_ejected")]
        public int MissionsHeadOfStateVIPEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_HeadOfState_bulk_delivered")]
        public int MissionsHeadOfStateBulkDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_HeadOfState_bulk_ejected")]
        public int MissionsHeadOfStateBulkEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Criminal_vip_delivered")]
        public int MissionsCriminalVIPDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Criminal_vip_ejected")]
        public int MissionsCriminalVIPEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Criminal_bulk_delivered")]
        public int MissionsCriminalBulkDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Criminal_bulk_ejected")]
        public int MissionsCriminalBulkEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Medical_vip_delivered")]
        public int MissionsMedicalVIPDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Medical_vip_ejected")]
        public int MissionsMedicalVIPEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Medical_bulk_delivered")]
        public int MissionsMedicalBulkDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Medical_bulk_ejected")]
        public int MissionsMedicalBulkEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Disgruntled_vip_delivered")]
        public int MissionsDisgruntledVIPDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Disgruntled_vip_ejected")]
        public int MissionsDisgruntledVIPEjected { get; init; }

        [JsonPropertyName("Passengers_Missions_Disgruntled_bulk_delivered")]
        public int MissionsDisgruntledBulkDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Disgruntled_bulk_ejected")]
        public int MissionsDisgruntledBulkEjected { get; init; }
    }
}
