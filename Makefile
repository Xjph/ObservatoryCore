DOTNET ?= dotnet

OPTS    := -c Proton --os win --arch x64 -p:EnableWindowsTargeting=true
PLUGINS := ObservatoryBotanist ObservatoryExplorer
PROJS   := ObservatoryFramework ObservatoryCore $(PLUGINS)
PUB_DIR := bin

.PHONY: all
all: build publish

# $1: proj
# $2: dotnet <TARGET>
# $3: dotnet TARGET <OPTS>
define subproj
.PHONY: $2/$1
$2/$1:
	$(DOTNET) $2 $(OPTS) $3 $1/$1.csproj

endef

# $1: PROJS[]
# $2: dotnet <TARGET>
# $3: dotnet TARGET <OPTS>
define subprojs

$(foreach p,$1,$(call subproj,$p,$2,$3))

endef

$(eval $(call subprojs,$(PROJS),build))
$(eval $(call subprojs,$(PLUGINS),publish,-o $(PUB_DIR)/plugins))
$(eval $(call subproj,ObservatoryCore,publish,--sc -p:PublishSingleFile=true -o $(PUB_DIR)))

.PHONY: build
build: $(addprefix build/,$(PROJS))
	
.PHONY: publish
publish: build $(addprefix publish/,ObservatoryCore $(PLUGINS))
	rm -rf $(PUB_DIR)/plugins/ObservatoryFramework.*
	mkdir -p $(PUB_DIR)/plugins/deps
	mv -t $(PUB_DIR)/plugins/deps $(PUB_DIR)/plugins/{KeraLua,lua54,NLua}.dll

.PHONY: clean
clean:
	rm -vrf publish $(addsuffix /bin,$(PROJS)) $(addsuffix /obj,$(PROJS))

.PHONY: winesetup
winesetup:
	winetricks -q dotnetdesktop8 ie8
    winetricks -q win10
	WINEDEBUG=fixme-all wine regedit wine-dll-overrides.reg
