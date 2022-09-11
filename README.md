# PartySampleFall2022

DONT PANIC AND MAKE GAMES !!!

# About the Base Project

In this semester, we will be developing a 2D platfomer together. The base project includes a controllable character, and an sample scene to play with. We will use this project as a sample to learn how to make games in Unity. At the end of the semester, we will (hopefully) have our own unique jumper game done.

![Base Project Screenshot](https://s2.loli.net/2022/09/11/Cn2kGtaPMWeB1TJ.png)

## Preparation

Before we actually take a step into the game development, here are something you should do to get ready!

0. Create a Unity ID at https://id.unity.com/.
1. Download and install UnityHub from https://unity3d.com/get-unity/download. Be aware that UnityHub is not the **Unity3D GameEngine**, it is a software that helps you download and organize your projects and engines of different versions.
2. Download Unity **2022.1.0f1** (make sure you have correct version number!) via UnityHub. We will use this version of the engine to develop our games. There is no restriction on what modules you should install.
3. Download GitHub desktop from https://desktop.github.com/. We will mainly use GitHub as our version control system. If you do not have a GitHub account, please sign up. If you have problems with GitHub, you can always discord me anytime.
4. The base project is here https://github.com/GuardHei/PartySampleFall2022. Make sure you have forked the project (ATTENTION: Please do FORK instead of CLONE).
5. IDE or coding editor suggestions: I personally highly recommend Rider from JetBrains. It has many features that specifically designed for Unity development. Also, it is from the same company that makes Intellij Idea (If you have taken CS61B, you know what I'm talking about). Most importantly it is free. All you need to do is to apply for an educational account. Other options including VS, VSCode are nice choices too.

## An Intro to Unity:

Check out the document here: https://docs.google.com/document/d/1v7z0DdEw7tjqu52R-4adhVVC3oGLB7_nJyqyeO_OSnI/edit?usp=sharing

## Project Structure:

The **Assets** folder under the root directory is where we store assets created for this project, including scripts, sprites, sfx, and etc. You will see different sub-folders inside, with names like **Prefabs**, **Scenes**, **Scripts** and etc. The name of the folder indicates what content should this folder hold. For example, we put all the sfx in the **Audios** folder, and all the scripts into the **Scripts** folder. It will be easy for other people to check your work!

### Tests Folder

There is one special folder, the **Tests** folder. It is a folder that will never synchronize with the repository on GitHub. In another way of saying, it is completely local, so you can put all your personal testing assets here (those you don't need other people to see). For example, if you want to mess up with an existing level of the game, just copy that level and move it to the **Tests** folder. Then you can do whatever you want to that file, without actually changing the project.

If you couldn't find the **Tests** folder, create one under the **Assets** directiory. This might happen because GitHub doesn't sync empty folders.