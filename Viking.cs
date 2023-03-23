using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using MelonLoader;
using SC2ExpansionLoader;
using static SC2ExpansionLoader.ModMain;
using Il2CppAssets.Scripts.Models.Towers.Upgrades;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models;
using UnityEngine;
using Il2Cpp;
using MelonLoader.Utils;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Utils;
using Il2CppAssets.Scripts.Models.Effects;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities;

[assembly:MelonGame("Ninja Kiwi","BloonsTD6")]
[assembly:MelonInfo(typeof(Viking.ModMain),Viking.ModHelperData.Name,Viking.ModHelperData.Version,"Silentstorm")]
[assembly:MelonOptionalDependencies("SC2ExpansionLoader")]
namespace Viking{
    public class ModMain:MelonMod{
        public static string LoaderPath=MelonEnvironment.ModsDirectory+"/SC2ExpansionLoader.dll";
        public override void OnEarlyInitializeMelon(){
            if(!File.Exists(LoaderPath)){
                var stream=new HttpClient().GetStreamAsync("https://github.com/Onixiya/SC2Expansion/releases/latest/download/SC2ExpansionLoader.dll");
                var fileStream=new FileStream(LoaderPath,FileMode.CreateNew);
                stream.Result.CopyToAsync(fileStream);
                Log("Exiting game so MelonLoader correctly loads all mods");
                Application.Quit();
            } 
        }
    }
    public class Viking:SC2Tower{
        public override string Name=>"Viking";
        public override string Description=>"Terran transforming mech, can attack normal bloons on the ground and Moab class bloons in the air";
        public override Faction TowerFaction=>Faction.Terran;
        public override UpgradeModel[]GenerateUpgradeModels(){
            return new UpgradeModel[]{
                new("Fighter Mode",900,0,new(){guidRef="Ui[Viking-AirIcon]"},0,0,0,"","Fighter Mode"),
                new("Phobos Weapons Systems",2025,0,new(){guidRef="Ui[Viking-PhobosWeaponsIcon]"},0,1,0,"","Phobos Weapons Systems"),
                new("Deimos",4150,0,new(){guidRef="Ui[Viking-DeimosGroundIcon]"},0,2,0,"","Deimos"),
                new("Sky Fury",11140,0,new(){guidRef="Ui[Viking-SkyFuryGroundIcon]"},0,3,0,"","Sky Fury"),
                new("Archangel",32670,0,new(){guidRef="Ui[Viking-ArchangelIcon]"},0,4,0,"","Archangel")
            };
        }
        public override int MaxTier=>5;
        public override Dictionary<string,Il2CppSystem.Type>Components=>new(){{"Viking-Prefab",Il2CppType.Of<VikingCom>()}};
		[RegisterTypeInIl2Cpp]
        public class VikingCom:MonoBehaviour{
            public VikingCom(IntPtr ptr):base(ptr){}
			public GameObject activeObj=null;
			public GameObject viking=null;
			public GameObject vikingAir=null;
			public GameObject deimos=null;
			public GameObject deimosAir=null;
			public GameObject skyFury=null;
			public GameObject skyFuryAir=null;
			public GameObject archangel=null;
			public Tower tower=null;
            float timer=0;
			int selectSound=0;
			int upgradeSound=0;
			void Start(){
				viking=transform.GetChild(0).gameObject;
				vikingAir=transform.GetChild(1).gameObject;
				deimos=transform.GetChild(2).gameObject;
				deimosAir=transform.GetChild(3).gameObject;
				skyFury=transform.GetChild(4).gameObject;
				skyFuryAir=transform.GetChild(5).gameObject;
				archangel=transform.GetChild(6).gameObject;
				vikingAir.SetActive(false);
				deimos.SetActive(false);
				deimosAir.SetActive(false);
				skyFury.SetActive(false);
				skyFuryAir.SetActive(false);
				archangel.SetActive(false);
				viking.transform.localPosition=new(0,0,0);
				vikingAir.transform.localPosition=new(0,0,0);
				deimos.transform.localPosition=new(0,0,0);
				deimosAir.transform.localPosition=new(0,0,0);
				skyFury.transform.localPosition=new(0,0,0);
				skyFuryAir.transform.localPosition=new(0,0,0);
				archangel.transform.localPosition=new(0,0,0);
				activeObj=viking;
			}
            void Update(){
                timer+=Time.fixedDeltaTime;
                if(timer>10){
                    if(activeObj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Viking-Stand")){
                        switch(new System.Random().Next(1,101)){
                            case<16:
                                PlayAnimation(activeObj.GetComponent<Animator>(),"Viking-Fidget",0.4f);
                                break;
                        }
                    }
                    timer=0;
                }
            }
			public void PlaySelectSound(){
				if(selectSound>5){
					selectSound=0;
				}
				selectSound+=1;
				PlaySound("Viking-Select"+selectSound);
			}
			public void PlayUpgradeSound(){
				upgradeSound+=1;
				selectSound=0;
				PlaySound("Viking-Upgrade"+upgradeSound);
			}
        }
        public override ShopTowerDetailsModel ShopDetails(){
            ShopTowerDetailsModel details=Game.instance.model.towerSet[0].Clone<ShopTowerDetailsModel>();
            details.towerId=Name;
            details.name=Name;
            details.towerIndex=13;
            details.pathOneMax=5;
            details.pathTwoMax=0;
            details.pathThreeMax=0;
            details.popsRequired=0;
            LocManager.textTable.Add("Fighter Mode Description","Trains the pilot to transform the Viking into a fighter jet gaining bonus damage against Moab's. Only targets Moab's in Fighter Mode");
            LocManager.textTable.Add("Phobos Weapons Systems Description","Increases range and damage in both modes");
            LocManager.textTable.Add("Deimos Description","Modified with questionable mercenary equipment. More pierce, damage, faster firing and can pop Lead Bloons");
            LocManager.textTable.Add("Sky Fury Description","Doubles damage dealt for 10 seconds after transforming");
            LocManager.textTable.Add("Archangel Description","Pride of the Dominion Engineering Corps, a Archangel is the solution to almost any threat");
            return details;
        }
        public override TowerModel[]GenerateTowerModels(){
            return new TowerModel[]{
                Base(),
                FighterMode(),
                PWS(),
                Deimos(),
                SkyFury(),
                Archangel()
            };
        }
        public TowerModel Base(){
            TowerModel viking=gameModel.GetTowerFromId("DartMonkey").Clone<TowerModel>();
            viking.name=Name;
            viking.baseId=viking.name;
            viking.towerSet=TowerSet.Magic;
            viking.cost=750;
            viking.tier=0;
            viking.tiers=new[]{0,0,0};
            viking.upgrades=new UpgradePathModel[]{new("Fighter Mode",Name+"-100")};
            viking.range=45;
            viking.radius=8;
            viking.display=new(){guidRef="Viking-Prefab"};
            viking.icon=new(){guidRef="Ui[Viking-GroundIcon]"};
            viking.instaIcon=new(){guidRef="Ui[Viking-GroundIcon]"};
            viking.portrait=new(){guidRef="Ui[Viking-GroundPortrait]"};
            viking.behaviors.GetModel<DisplayModel>().display=viking.display;
            AttackModel gatling=viking.behaviors.GetModel<AttackModel>();
			gatling.name="Viking-Gatling";
            gatling.range=viking.range;
            AttackFilterModel attackFilter=gatling.behaviors.GetModel<AttackFilterModel>();
            List<FilterModel>attackFilters=attackFilter.filters.ToList();
            attackFilters.Add(new FilterOutTagModel("MoabFilter","Moabs",null));
            attackFilter.filters=attackFilters.ToArray();
            WeaponModel weapon=gatling.weapons[0];
            weapon.ejectX=7.5f;
            weapon.ejectY=10;
            weapon.rate=0.95f;
            weapon.emission=new InstantDamageEmissionModel("IDEM",null);
            PrefabReference effectDisplay=new(){guidRef="ebbd125f548044f4f9a12618c4ed9c17"};
            weapon.behaviors=new WeaponBehaviorModel[]{new EjectEffectModel("EjectEffectModel",effectDisplay,
                new EffectModel("EffectModel",effectDisplay,1,0.2f,false,false,false,false,false,false,false),
                0.2f,false,true,true,false,false)};
            ProjectileModel proj=weapon.projectile;
            proj.display=new(){guidRef=""};
            proj.pierce=1;
            List<Model>projBehav=proj.behaviors.ToList();
            DamageModel damage=projBehav.GetModel<DamageModel>();
            damage.damage=2;
            damage.immuneBloonProperties=(BloonProperties)17;
            projBehav.Add(new InstantModel("InstantModel",true));
            List<WeaponModel>weapons=gatling.weapons.ToList();
            weapons.Add(weapon.Clone<WeaponModel>());
            weapons[1].ejectX=-gatling.weapons[0].ejectX;
            gatling.weapons=weapons.ToArray();
            return viking;
        }
        public TowerModel AirMode(){
            TowerModel viking=gameModel.GetTowerFromId("DartMonkey").Clone<TowerModel>();
            viking.name=Name+"-Air100";
            viking.baseId=viking.name;
            viking.towerSet=TowerSet.Magic;
            viking.tier=1;
            viking.tiers=new[]{1,0,0};
			viking.display=new(){guidRef="Viking-AirPrefab"};
            viking.portrait=new(){guidRef="Ui[Viking-AirIcon]"};
            viking.appliedUpgrades=new(new[]{"Fighter Mode"});
            viking.upgrades=new UpgradePathModel[0];
            viking.range=90;
            DisplayModel display=viking.behaviors.GetModel<DisplayModel>();
            display.positionOffset=new(0,0,190);
			display.display=viking.display;
            AttackModel torpedoes=viking.behaviors.GetModel<AttackModel>();
            torpedoes.name="VikingTorpedoes";
            torpedoes.range=viking.range;
            torpedoes.addsToSharedGrid=false;
            torpedoes.attackThroughWalls=true;
            AttackFilterModel attackFilter=torpedoes.behaviors.GetModel<AttackFilterModel>();
            List<FilterModel>filters=attackFilter.filters.ToList();
            filters.Add(new FilterWithTagModel("FilterWithTagModel","Moabs",false));
            attackFilter.filters=filters.ToArray();
            WeaponModel torpedoesWep=torpedoes.weapons[0];
            torpedoesWep.rate=0.8f;
            ProjectileModel torpedoesProj=torpedoesWep.projectile;
            torpedoesProj.display=new(){guidRef="Viking-MissilePrefab"};
            torpedoesProj.pierce=1;
            torpedoesProj.filters=attackFilter.filters;
            DamageModel projDamage=torpedoesProj.behaviors.GetModel<DamageModel>();
            projDamage.damage=5;
            projDamage.immuneBloonProperties=(BloonProperties)2;
            torpedoesProj.behaviors.GetModel<TravelStraitModel>().speed=700;
            AbilityModel transform=BlankAbilityModel.Clone<AbilityModel>();
            transform.name="VikingAirTransform";
            transform.icon=new(){guidRef="Ui[Viking-GroundIcon]"};
            transform.cooldown=1;
            List<Model>transformBehav=transform.behaviors.ToList();
            transformBehav.Add(new TurboModel("VikingAirTransform",1,1,null,0,0,false));
            transform.behaviors=transformBehav.ToArray();
            List<Model>vikingBehav=viking.behaviors.ToList();
            vikingBehav.Add(transform);
            viking.behaviors=vikingBehav.ToArray();
            return viking;
        }
        public TowerModel FighterMode(){
            TowerModel viking=Base().Clone<TowerModel>();
            viking.name=Name+"-100";
            viking.tier=1;
            viking.tiers=new[]{1,0,0};
            viking.appliedUpgrades=new(new[]{"Fighter Mode"});
            viking.upgrades=new UpgradePathModel[]{new("Phobos Weapons Systems",Name+"-200")};
            AbilityModel transform=BlankAbilityModel.Clone<AbilityModel>();
            transform.name="VikingTransform";
            transform.cooldown=1;
            transform.icon=new(){guidRef="Ui[Viking-AirIcon]"};
            transform.addedViaUpgrade="Fighter Mode";
			List<Model>vikingBehav=viking.behaviors.ToList();
			AttackModel torpedoes=vikingBehav.GetModel<AttackModel>().Clone<AttackModel>();
            torpedoes.name="VikingTorpedoes";
            torpedoes.range=viking.range;
            torpedoes.addsToSharedGrid=false;
            torpedoes.attackThroughWalls=true;
            AttackFilterModel attackFilter=torpedoes.behaviors.GetModel<AttackFilterModel>();
            List<FilterModel>filters=attackFilter.filters.ToList();
            filters.Add(new FilterWithTagModel("FilterWithTagModel","Moabs",true));
            attackFilter.filters=filters.ToArray();
            WeaponModel torpedoesWep=torpedoes.weapons[0];
            torpedoesWep.rate=0.8f;
            ProjectileModel torpedoesProj=torpedoesWep.projectile;
            torpedoesProj.display=new(){guidRef="Viking-MissilePrefab"};
            torpedoesProj.pierce=1;
            torpedoesProj.filters=attackFilter.filters;
            DamageModel projDamage=torpedoesProj.behaviors.GetModel<DamageModel>();
            projDamage.damage=5;
            projDamage.immuneBloonProperties=(BloonProperties)2;
            torpedoesProj.behaviors.GetModel<TravelStraitModel>().speed=700;
            List<Model>transformBehav=transform.behaviors.ToList();
            transformBehav.Add(new ActivateAttackModel("Viking-Transform",5,false,new(new[]{torpedoes}),false,true,false,false,false));
            transform.behaviors=transformBehav.ToArray();
            vikingBehav.Add(transform);
            viking.behaviors=vikingBehav.ToArray();
            return viking;
        }
        public TowerModel PWS(){
            TowerModel viking=FighterMode().Clone<TowerModel>();
            viking.name=Name+"-200";
            viking.tier=2;
            viking.tiers=new[]{2,0,0};
            viking.range+=10;
            viking.appliedUpgrades=new(new[]{"Fighter Mode","Phobos Weapons Systems"});
            viking.upgrades=new UpgradePathModel[]{new("Deimos",Name+"-300")};
            AttackModel gatling=viking.behaviors.GetModel<AttackModel>();
            gatling.range=viking.range;
            foreach(WeaponModel weapon in gatling.weapons){
                weapon.projectile.behaviors.GetModel<DamageModel>().damage+=2;
            }
            AttackModel torpedos=viking.behaviors.GetModel<AbilityModel>().behaviors.GetModel<ActivateAttackModel>().attacks[0];
			torpedos.range=viking.range;
			torpedos.GetDescendant<DamageModel>().damage+=5;
            return viking;
        }
        public TowerModel Deimos(){
            TowerModel viking=PWS().Clone<TowerModel>();
            viking.name=Name+"-300";
            viking.tier=3;
            viking.tiers=new[]{3,0,0};
            viking.portrait=new(){guidRef="Ui[Viking-DeimosGroundPortrait]"};
            viking.behaviors.GetModel<DisplayModel>().display=viking.display;
            viking.appliedUpgrades=new(new[]{"Fighter Mode","Phobos Weapons Systems","Deimos"});
            viking.upgrades=new UpgradePathModel[]{new("Sky Fury",Name+"-400")};
			Il2CppReferenceArray<Model>vikingBehav=viking.behaviors;
            AbilityModel transform=vikingBehav.GetModel<AbilityModel>();
            transform.icon=new(){guidRef="Ui[Viking-DeimosAirIcon]"};
			ActivateAttackModel transformAttack=transform.GetDescendant<ActivateAttackModel>();
			List<AttackModel>transformAttacks=transformAttack.attacks.ToList();
            AttackModel wild=transformAttacks[0].Clone<AttackModel>();
            wild.name="WILD";
            wild.range+=50;
            AttackFilterModel filter=wild.behaviors.GetModel<AttackFilterModel>();
            List<FilterModel>filters=filter.filters.ToList();
            filters.Add(new FilterWithTagModel("FilterWithTagModel","Moabs",true));
            filter.filters=filters.ToArray();
            WeaponModel wildWeapon=wild.weapons[0];
            wildWeapon.emission=new RandomArcEmissionModel("RandomArcEmissionModel",8,30,90,10,10,null);
            wildWeapon.rate=1.05f;
            ProjectileModel wildProj=wildWeapon.projectile;
            wildProj.display=new(){guidRef="Viking-MissilePrefab"};
            List<Model>projBehav=wildProj.behaviors.ToList();
            projBehav.Remove(projBehav.First(a=>a.GetIl2CppType().Name=="TravelStraitModel"));
            projBehav.Add(new TravelCurvyModel("TravelCurvyModel",200,5,720,45));
            wildProj.behaviors=projBehav.ToArray();
			transformAttacks.Add(wild);
			transformAttack.attacks=transformAttacks.ToArray();
            foreach(WeaponModel weapon in vikingBehav.GetModel<AttackModel>().weapons){
                weapon.rate=0.55f;
                ProjectileModel proj=weapon.projectile;
                proj.pierce=3;
                DamageModel damageModel=proj.behaviors.GetModel<DamageModel>();
                damageModel.damage+=1;
                damageModel.immuneBloonProperties=(BloonProperties)16;
            }
            return viking;
        }
        public TowerModel SkyFury(){
            TowerModel viking=Deimos().Clone<TowerModel>();
            viking.name=Name+"-400";
            viking.tier=4;
            viking.tiers=new[]{4,0,0};
            viking.portrait=new(){guidRef="Ui[Viking-SkyFuryGroundPortrait]"};
            viking.appliedUpgrades=new(new[]{"Fighter Mode","Phobos Weapons Systems","Deimos","Sky Fury"});
            viking.upgrades=new UpgradePathModel[]{new("Archangel",Name+"-500")};
            AbilityModel transform=viking.behaviors.GetModel<AbilityModel>();
            transform.icon=new(){guidRef="Ui[Viking-SkyFuryAirIcon]"};
            List<Model>transformBehav=transform.behaviors.ToList();
            transformBehav.Add(new TurboModel("TurboModel",10,2,new("AssetPath",new(){guidRef="Viking-MissilePrefab"}),0,1.2f,false));
            return viking;
        }
        public TowerModel Archangel(){
            TowerModel viking=SkyFury().Clone<TowerModel>();
            viking.name=Name+"-500";
            viking.tier=5;
            viking.tiers=new[]{5,0,0};
            viking.portrait=new(){guidRef="Ui[Viking-ArchangelPortrait]"};
            viking.appliedUpgrades=new(new[]{"Fighter Mode","Phobos Weapons Systems","Deimos","Sky Fury","Archangel"});
            viking.upgrades=new UpgradePathModel[0];
            viking.range=100;
            List<Model>vikingBehav=viking.behaviors.ToList();
            AttackModel gatling=vikingBehav.GetModel<AttackModel>();
            gatling.range=viking.range;
            AttackFilterModel filter=gatling.behaviors.GetModel<AttackFilterModel>();
            List<FilterModel>filters=filter.filters.ToList();
            filters.Remove(filters.First(a=>a.name.Contains("MoabFilter")));
            filter.filters=filters.ToArray();
            foreach(WeaponModel weapon in gatling.weapons){
                weapon.rate=0.35f;
                EjectEffectModel effect=weapon.behaviors[0].Cast<EjectEffectModel>();
                effect.effectModel.assetId=new(){guidRef="643075fcdb276d34596b8a6386b59ca8"};
                effect.assetId=effect.effectModel.assetId;
                DamageModel damage=weapon.projectile.behaviors.GetModel<DamageModel>();
                damage.damage=11;
                damage.immuneBloonProperties=0;
            }
            AttackModel wild=vikingBehav.GetModel<AbilityModel>().behaviors.GetModel<ActivateAttackModel>().attacks[1].Clone<AttackModel>();
            wild.range=viking.range+20;
            wild.weapons[0].emission=new RandomArcEmissionModel("RandomArcEmissionModel",16,0,45,0,10,null);
            wild.weapons[0].projectile.behaviors.GetModel<TravelCurvyModel>().maxTurnAngle=30;
            wild.weapons[0].projectile.behaviors.GetModel<DamageModel>().damage*=2;
            wild.weapons[0].projectile.behaviors.GetModel<DamageModel>().immuneBloonProperties=0;
            vikingBehav.Remove(vikingBehav.First(a=>a.GetIl2CppType().Name=="AbilityModel"));
            vikingBehav.Add(wild);
            viking.behaviors=vikingBehav.ToArray();
            return viking;
        }
        public override void Create(Tower tower){
            PlaySound("Viking-Birth");
        }
        public override void Upgrade(int tier,Tower tower){
			VikingCom com=tower.Node.graphic.GetComponent<VikingCom>();
			if(com.tower==null){
				com.tower=tower;
			}
            switch(tier){
                case 3:
					com.activeObj.SetActive(false);
					com.activeObj=com.deimos;
					com.activeObj.SetActive(true);
                    break;
                case 4:
					com.activeObj.SetActive(false);
					com.activeObj=com.skyFury;
					com.activeObj.SetActive(true);
                    break;
                case 5:
					com.activeObj.SetActive(false);
					com.activeObj=com.archangel;
					com.activeObj.SetActive(true);
                    break;
			}
            com.PlayUpgradeSound();
        }
        public override void Select(Tower tower){
            tower.Node.graphic.gameObject.GetComponent<VikingCom>().PlaySelectSound();
        }
        public override void Attack(Weapon weapon){
            PlayAnimation(weapon.attack.tower.Node.graphic.GetComponent<VikingCom>().activeObj.GetComponent<Animator>(),"Viking-Attack");
        }
        public override bool Ability(string ability,Tower tower){
            if(ability=="VikingTransform"){
                VikingCom com=tower.Node.graphic.GetComponent<VikingCom>();
				com.activeObj.SetActive(false);
				switch(tower.towerModel.tier){
					case 1 or 2:
						com.activeObj=com.vikingAir;
						break;
					case 3:
						com.activeObj=com.deimosAir;
						break;
					case 4:
						com.activeObj=com.skyFuryAir;
						break;
				}
				com.activeObj.SetActive(true);
				com.tower.attackBehaviorsInDependants[0].attack.range=0;
				tower.Position.Z=190;
            }
			return true;
        }
		[HarmonyPatch(typeof(ActivateAttack),"RemoveProcess")]
		public class ActivateAttackRemoveProcess_Patch{
			public static void Postfix(ref ActivateAttack __instance){
				Ability ability=__instance.entity.dependants[0].Cast<Ability>();
				if(ability.abilityModel.name=="VikingTransform"){
					VikingCom com=ability.tower.Node.graphic.GetComponent<VikingCom>();
					com.activeObj.SetActive(false);
					switch(ability.tower.towerModel.tier){
						case 1 or 2:
							com.activeObj=com.viking;
							break;
						case 3:
							com.activeObj=com.deimos;
							break;
						case 4:
							com.activeObj=com.skyFury;
							break;
					}
					com.activeObj.SetActive(true);
					com.tower.attackBehaviorsInDependants[0].attack.range=com.tower.towerModel.behaviors.GetModel<AttackModel>().range;
					ability.tower.Position.Z=0;
				}
			}
		}
    }
}