$MusicPlayer::Root = "Add-Ons/Client_MusicPlayer/";

$remapDivision[$remapCount] = "Music Player";
$remapName[$remapCount] = "Open";
$remapCmd[$remapCount] = "MusicPlayer_pushDialog";
$remapCount++;

function MusicPlayer_pushDialog()
{
        
        canvas.pushDialog(MusicPlayerGUI);

}

if(isObject(MusicPlayerGUI))
{

	MusicPlayerGUI.delete();
	exec($MusicPlayer::Root @ "MusicPlayerGUI.gui");

}
else
{

	exec($MusicPlayer::Root @ "MusicPlayerGUI.gui");

}

new AudioDescription(MusicPlayerAudio)
{
        is3D = 0;
        isLooping = 1;
        loopcount = -1;
        type = 8;
        volume = 2;
};

function populateMusicList()
{

	if($MusicPlayer::SetModPaths) { setModPaths(getModPaths()); }

	if(isObject(MusicList))
	{

		for(%i=0;%i<MusicList.getCount();%i++)
		{

			%obj = MusicList.getObject(%i);
			%obj.delete();

		}

		MusicList.delete();

	}

	%list = new SimSet(MusicList)
	{

		name = "MusicList";

	};

	%file = findFirstFile("Add-Ons/Music/*.ogg");

	while(%file !$= "")
	{

		//let's get only the file and not the folder
		%name = strReplace(%file,"Add-Ons/Music/","");
		%name = strReplace(%name,"_"," ");
		%name = strReplace(%name,".ogg","");

		%data = new ScriptObject(MusicListData)
		{

			filename = %file;
			name = %name;

		};

		if(isFile(%file)) { MusicList.add(%data); }

    	%file = findNextFile("Add-Ons/Music/*.ogg");

	}

	%file = findFirstFile("Add-Ons/Music_MusicPlayer/*.ogg");

	while(%file !$= "")
	{

		//let's get only the file and not the folder
		%name = strReplace(%file,"Add-Ons/Music_MusicPlayer/","");
		%name = strReplace(%name,"_"," ");
		%name = strReplace(%name,".ogg","");

		%data = new ScriptObject(MusicListData)
		{

			filename = %file;
			name = %name;

		};

		if(isFile(%file)) { MusicList.add(%data); }
		
    	%file = findNextFile("Add-Ons/Music_MusicPlayer/*.ogg");

	}

	//now to populate the GUI list
	%listObj = MusicPlayerList;
	%listObj.clear();

	if(isObject(MusicList))
	{
	
		for(%i=0;%i<MusicList.getCount();%i++)
		{
	
			%line = MusicList.getObject(%i).name;
			%listObj.addRow(%i,%line);
	
		}
	
	}
	else
	{
	
		error("Music database non-existant!");
	
	}

}

function getMusicListObject()
{

	%rowID = MusicPlayerList.getSelectedID();
	%word = MusicPlayerList.getRowTextByID(%rowID);

	for(%i=0;%i<MusicList.getCount();%i++)
	{

		%obj = MusicList.getObject(%i);
		if(%obj.name $= %word)
		{

			return(%obj);
			break;

		}

	}

}

function clickMusicListObject()
{

	alxStop($MusicPlayer::CurrentMusic);

	%music = getMusicListObject();

	$MusicPlayer::CurrentMusic = alxCreateSource(MusicPlayerAudio, %music.filename);

	alxPlay($MusicPlayer::CurrentMusic);

}

function changeMusicPlayerVolume()
{

	MusicPlayerAudio.volume = MusicPlayerVolume.getValue();

	clickMusicListObject();

}

function MusicPlayerGUI::onWake()
{

	populateMusicList();

}

package MusicPlayerPackage
{

	function PlayGui::onWake(%this,%obj)
	{

		parent::onWake(%this,%obj);

		$MusicPlayer::SetModPaths = 0;

	}

	function MainMenuButtonsGui::onWake(%this,%obj)
	{

		$MusicPlayer::SetModPaths = 1;
		setModPaths(getModPaths());

	}

};
activatePackage(MusicPlayerPackage);