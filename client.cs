// echo(alxGetSourcef($MusicPlayer::CurrentMusic, "AL_PITCH"));
// alxSourcef($MusicPlayer::CurrentMusic, "AL_PITCH", 0.5);

$remapDivision[$remapCount] = "Music Player";
$remapName[$remapCount] = "Open";
$remapCmd[$remapCount] = "MusicPlayer_pushDialog";
$remapCount++;

function MusicPlayer_pushDialog() {
	canvas.pushDialog(MusicPlayerGUI);
}

if(isObject(MusicPlayerGUI)) {
	MusicPlayerGUI.delete();
}
exec("./MusicPlayerGUI.gui");

new AudioDescription(MusicPlayerAudio) {
	is3D = 0;
	isLooping = 1;
	loopcount = -1;
	type = 8;
	volume = 2;
};

function populateMusicList() {
	if($MusicPlayer::SetModPaths) {
		setModPaths(getModPaths());
	}

	if(isObject(MusicList)) {
		for(%i=0;%i<MusicList.getCount();%i++){
			MusicList.getObject(0).delete();
		}

		MusicList.delete();
	}

	%list = new SimSet(MusicList) {
		name = "MusicList";
	};

	%patterns = "Add-Ons/Music/*.ogg\tAdd-Ons/Music_MusicPlayer/*.ogg\tconfig/client/music/*.ogg";
	for(%i=0;%i<getFieldCount(%patterns);%i++) {
		%pattern = getField(%patterns, %i);
		echo("Checking" SPC %pattern);

		for(%file = findFirstFile(%pattern); isFile(%file); %file = findNextFile(%pattern)) {
			%data = new ScriptObject(MusicListData) {
				filename = %file;
				name = strReplace(fileBase(%file),"_"," ");
			};
			%list.add(%data);
		}
	}

	//now to populate the GUI list
	%listObj = MusicPlayerList;
	%listObj.clear();

	if(isObject(MusicList)) {
		for(%i=0;%i<MusicList.getCount();%i++) {
			%line = MusicList.getObject(%i).name;
			%listObj.addRow(%i,%line);
		}

		%listObj.sort(0);
	} else {
		error("Music database non-existant!");
	}
}

function getMusicListObject() {
	%rowID = MusicPlayerList.getSelectedID();
	%word = MusicPlayerList.getRowTextByID(%rowID);

	for(%i=0;%i<MusicList.getCount();%i++) {
		%obj = MusicList.getObject(%i);
		if(%obj.name $= %word) {
			return(%obj);
		}
	}
}

function clickMusicListObject() {
	alxStop($MusicPlayer::CurrentMusic);
	%music = getMusicListObject();
	$MusicPlayer::CurrentMusic = alxCreateSource(MusicPlayerAudio, %music.filename);
	alxPlay($MusicPlayer::CurrentMusic);
	changeMusicPlayerVolume(1);
	changeMusicPlayerPitch(1);
}

function changeMusicPlayerVolume(%box) {
	if(%box) {
		%value = MusicPlayerVolumeValue.getValue();
		MusicPlayerVolume.setValue(%value);
	} else {
		%value = MusicPlayerVolume.getValue();
		MusicPlayerVolumeValue.setText(%value);
	}

	alxSourcef($MusicPlayer::CurrentMusic, "AL_GAIN_LINEAR", %value);
}

function changeMusicPlayerPitch(%box) {
	if(%box) {
		%value = MusicPlayerPitchValue.getValue();
		MusicPlayerPitch.setValue(%value);
	} else {
		%value = MusicPlayerPitch.getValue();
		MusicPlayerPitchValue.setText(%value);
	}

	alxSourcef($MusicPlayer::CurrentMusic, "AL_PITCH", %value);
}

function resetMusicParams() {
	MusicPlayerPitchValue.setText(1);
	MusicPlayerVolumeValue.setText(1);

	changeMusicPlayerPitch(1);
	changeMusicPlayerVolume(1);
}

function MusicPlayerGUI::onWake() {
	populateMusicList();
}

package MusicPlayerPackage {
	function PlayGui::onWake(%this,%obj) {
		parent::onWake(%this,%obj);
		$MusicPlayer::SetModPaths = false;
	}

	function MainMenuButtonsGui::onWake(%this,%obj) {
		parent::onWake(%this,%obj);
		$MusicPlayer::SetModPaths = true;
		setModPaths(getModPaths());
	}
};
activatePackage(MusicPlayerPackage);