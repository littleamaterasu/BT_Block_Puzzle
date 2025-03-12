import { _decorator, AudioClip, AudioSource, Component, resources } from 'cc';
import { BGSOUND_FILE_PATH, VOLUME } from './constant/constant';
const { ccclass, property } = _decorator;

@ccclass('AudioController')
export class AudioController extends Component {
    @property([AudioClip]) comboSounds: AudioClip[] = [];
    @property([AudioClip]) commonSounds: AudioClip[] = [];
    @property([AudioClip]) themeSounds: AudioClip[] = [];

    @property(AudioSource) music: AudioSource = null;
    @property(AudioSource) sound: AudioSource = null;

    private _isMuteMusic: boolean = false;
    private _soundVolume: number = 1.0; // Dùng thay cho _isMuteSoundEffect

    playSoundEffect(audioSource: AudioSource, soundEffects: AudioClip[], index: number) {
        if (soundEffects && soundEffects[index]) {
            audioSource.playOneShot(soundEffects[index], this._soundVolume);
        }
    }

    playBgSound() {
        resources.load(BGSOUND_FILE_PATH, AudioClip, (err, clip) => {
            if (err) {
                console.error("Failed to load background sound:", err);
                return;
            }
            this.music.clip = clip;
            this.music.loop = true;
            this.music.volume = this._isMuteMusic ? 0 : 0.5;
            this.music.play();
        });
    }

    playComboSound(index: number) {
        index = Math.min(index, 3);
        this.playSoundEffect(this.sound, this.comboSounds, index);
    }

    playCommonSound(index: number) {
        console.log('play sound');
        this.playSoundEffect(this.sound, this.commonSounds, index);
    }

    playThemeSound(index: number) {
        this.playSoundEffect(this.sound, this.themeSounds, index);
    }

    stopAllSounds() {
        this.sound.stop();
        this.music.stop();
    }

    toggleMusic() {
        this._isMuteMusic = !this._isMuteMusic;
        this.music.volume = this._isMuteMusic ? 0 : VOLUME.MUSIC;
    }

    toggleSound() {
        this._soundVolume = this._soundVolume === 0 ? VOLUME.SOUND : 0;
    }
}
