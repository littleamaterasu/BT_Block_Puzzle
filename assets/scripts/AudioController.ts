import { _decorator, AudioClip, AudioSource, Component, resources } from 'cc';
import { BGSOUND_FILE_PATH } from './constant/constant';
const { ccclass, property } = _decorator;

@ccclass('AudioController')
export class AudioController extends Component {
    @property([AudioClip]) comboSounds: AudioClip[] = [];

    @property([AudioClip]) commonSounds: AudioClip[] = [];
    
    @property([AudioClip]) themeSounds: AudioClip[] = [];

    @property(AudioSource) audioSource: AudioSource = null;

    playSoundEffect(soundEffects: AudioClip[], index: number, volume: number = 1.0){
        if(soundEffects && index && soundEffects[index]){
            this.audioSource.playOneShot(soundEffects[index], volume);
        }
    }

    // Loop, file path C:\Users\ADMIN\Desktop\BT_Block_Puzzle\assets\resources\block_puzzle\StarBox\Audio\themes\cute\bgm.mp3
    playBgSound() {
        resources.load(BGSOUND_FILE_PATH, AudioClip, (err, clip) => {
            if (!err) {
                this.audioSource.clip = clip;
                this.audioSource.loop = true; // Lặp lại
                this.audioSource.volume = 0.5; // Âm lượng vừa phải
                this.audioSource.play();
            }
        });
    }

    playComboSound(index: number){
        this.playSoundEffect(this.comboSounds, index);
    }

    playCommonSound(index: number){
        this.playSoundEffect(this.commonSounds, index);
    }
    
    playThemeSound(index: number){
        this.playSoundEffect(this.themeSounds, index);
    }

    stopAllSounds(){
        this.audioSource.stop();
    }
}


