# open-source-auto-pilot-Ardupilot-Mega-V2.5.2
Complete manual of Ardupilot Mega V2.5.2 with example application
.. _building-ardupilot-with-arduino-windows:


Archived: Building ArduPilot for APM2.x on Windows with Arduino
===============================================================

This article shows how to build ArduPilot for APM2.x targets on Windows,
using the Arduino toolchain.

.. tip:

   An alternative approach is covered in :ref:`Building ArduPilot for APM2.x on Windows with Make <building-ardupilot-for-apm2-x-on-windows-with-make>`.

.. warning:

   Copter 3.3 firmware (and later) and builds after Plane 3.4.0 no longer
   fit on APM boards. Plane, Rover and AntennaTracker builds can still be
   installed at time of writing but you can no longer build APM2.x off the
   master branch (you will need to build off a supported release branch).

   The last Copter firmware that can be built on APM 2.x `can be downloaded from here <https://github.com/ArduPilot/ardupilot/archive/master-AVR.zip>`__.

   In addition to the above restrictions, this article covers:

   -  From Copter version 3.1 to version 3.2.1
   -  From Plane version 2.76 to version 3.4.0
   -  APM 2.0, 2.5, and 2.6 only

Install Git-SCM
===============

1. Download and run the install file from:
   https://git-scm.com/download/win
2. Follow the screenshots below to make your selections during install.

![git_setup_wizard_welcome](https://user-images.githubusercontent.com/30057649/125969065-1365d055-f834-4675-9dd2-4b41664e6fe3.jpg)
       
       
![git_setup_wizard_license](https://user-images.githubusercontent.com/30057649/125969055-fc666497-bd51-4e76-badf-a802c8296a82.jpg)
    
3. On the *Welcome screen* and then again on the *License screen* click
   the **Next** button

![git_setup_wizard_components](https://user-images.githubusercontent.com/30057649/125969047-b7d86651-a37c-4e46-91a4-51e4ce18f145.jpg)
   
   
![git_setup_wizard_view_release_notes](https://user-images.githubusercontent.com/30057649/125969062-3985a263-ead7-4875-987e-2f37b3837da0.jpg)
   
4. On the *Select Components screen* click on the **Next** button, then
   click the **Finish** button

![git_setup_wizard_replace_in_use_files](https://user-images.githubusercontent.com/30057649/125969058-ba022006-4c10-4d26-80b8-a4b51a61d71a.jpg)
   
   
![git_setup_wizard_installing_files](https://user-images.githubusercontent.com/30057649/125969051-9e537abc-4fab-4878-9824-167ab8bb66b4.jpg)
   
5. Click the **Next** button in the *Replacing in Use Files Screen*,
   then wait for Git to finish loading

![git_setup_wizard_set_line_endings](https://user-images.githubusercontent.com/30057649/125969061-d546c406-2ccb-4a07-b216-382000792913.jpg)
   
   
![git_setup_wizard_run_git_command_prompt](https://user-images.githubusercontent.com/30057649/125969059-eb602bc1-8e59-4405-b957-c4cf24f4b2a8.jpg)
   
6. Select the *Checkout Windows* item and the **Next** button then
   Select the *Run Git from Windows* item and the **Next** button.

Download source
===============

1. In your C: drive, make a folder called GIT (**C:\\GIT** on my
   computer). Navigate to the folder Windows Explorer
   
![git_setup_create_git_directory](https://user-images.githubusercontent.com/30057649/125969045-63dca18d-3920-41d7-b2cc-663e7972792f.jpg)
   
2. Right click anywhere in the folder and click git bash

![git_bash_command_prompt](https://user-images.githubusercontent.com/30057649/125969039-2428f813-cffe-460c-b371-e8fac320f2e5.jpg)

   This screen will come up

![git_bash_prompt](https://user-images.githubusercontent.com/30057649/125969042-88827b49-e63f-456a-b0bf-0e8afee2cd98.jpg)

3. In this screen type

   :

       git clone https://github.com/ahmed24630/open-source-auto-pilot-Ardupilot-Mega-V2.5.2.git

![git_bash_clone_ardupilot](https://user-images.githubusercontent.com/30057649/125969036-3e2cefe4-fa44-43b2-823a-d6e029532d13.jpg)

   When it is finished it should look like this….

![git_bash_clone_ardupilot_complete](https://user-images.githubusercontent.com/30057649/125969038-50fc39f1-cb59-4a97-b2c2-2ac21bcf9ab3.jpg)

4. A little more initialisation is required for the source code.  Initialise referenced dependencies like this:

   :

      cd ardupilot
      git submodule update --init --recursive

Install MHV_AVR_Tools to its default location
=============================================

1. Download and install the MHV_AVR Tools:
   `https://firmware.ardupilot.org/Tools/Arduino/MHV_AVR_Tools_20121007.exe <https://firmware.ardupilot.org/Tools/Arduino/MHV_AVR_Tools_20131101.exe>`__

![mhv_avr_tools_installer_welcome](https://user-images.githubusercontent.com/30057649/125969079-df987e27-f509-4a13-8803-e6dcadfa648e.jpg)
   
![mhv_avr_tools_installer_choose_start_menu_folder](https://user-images.githubusercontent.com/30057649/125969072-85c6929d-1a91-4fdd-92f8-cc6e5ca624a6.jpg)
   
2. Select the **Next** button in the setup wizard screen then select the
   **Install** button for *MHV AVR Tools*

![mhv_avr_tools_installer_choose_components](https://user-images.githubusercontent.com/30057649/125969068-2117c304-d5a2-4500-afb7-2be51c2d1758.jpg)
   
![mhv_avr_tools_installer_choose_destination_folder](https://user-images.githubusercontent.com/30057649/125969069-922aab37-245b-4886-ab03-12df77d51483.jpg)
   
3. Check both items in the Choose *Components Screen* and select
   **Next** then select **Next** again to install to the default
   location

![mhv_avr_tools_installer_license_agreement](https://user-images.githubusercontent.com/30057649/125969075-92cb2216-c4d2-46d7-bf76-00c61734d3d5.jpg)
   
4. Select the **I Agree** button on the *License Agreement screen*.

Install ArduPilot-Arduino
=========================

Download and unzip the ArduPilot Arduino package:
https://firmware.ardupilot.org/Tools/Arduino/ArduPilot-Arduino-1.0.3-gcc-4.8.2-windows.zip

This can be unzipped directly to the **C:** drive or **C:\\Program
Files\\**

.. note::

   This is a special ArduPilot Arduino package which contains gcc
   4.8.2

Configure Arduino
=================

1. Go to your Arduino folder
   
2. Double click the Arduino icon

![arduino_icon](https://user-images.githubusercontent.com/30057649/125974790-7a655c08-a019-4286-826a-7c80bd005750.jpg)
   
3. When Arduino opens, go to the file menu


![arduino_menu_preferences](https://user-images.githubusercontent.com/30057649/125969020-7fc0623c-5e10-43a8-b92b-e2871c84eceb.jpg)
   
4. Select preferences


![arduino_preferences_dialog](https://user-images.githubusercontent.com/30057649/125969021-e0aa3ac9-1eaa-4e60-ac3b-639a957510db.png)

   -  Set Sketchbook location to your ArduPilot directory in your GIT
      folder.
   -  Set verbose for both compile and upload
   -  And DO NOT check for updates on start-up… (Remember, this is a
      special version just for ArduPilot.)

5. Click **OK** and close Arduino

Connect your APM to your USB
============================

4. Re-open ArduPilot and under the file tab, click on sketchbook, then
   the program you wish to load onto your APM2.x (for this example we
   will use Copter, though the others use the same methods.

![arduino_tools_sketchbook_copter](https://user-images.githubusercontent.com/30057649/125969030-3cf67e89-373f-4442-a485-7f914ecf2816.jpg)
   
2. Once this is loaded, click on the ArduPilot tab, and select ArduPilot
   mega 2.x out of the HAL options.

![arduino_tools_target_apm2](https://user-images.githubusercontent.com/30057649/125969032-530bda83-fe75-4abe-90de-64a8f00d905a.jpg)
   
3. Then click the “Tools” tab and select “Arduino Mega 2560 or Mega ADK”
   from the “Board” tab.

![arduino_tools_select_target_board](https://user-images.githubusercontent.com/30057649/125969026-bb3296f9-66ff-420d-aec9-b39c0045152b.jpg)
   
4. Next select the *Tools* tab again, and set the “Serial Port” to the
   one your APM is connected to.

![arduino_tools_serial_port](https://user-images.githubusercontent.com/30057649/125969028-fec5d76f-269a-4dfd-ac6c-4b6e8ea76611.jpg)
   
5. In my case it was COM4, but check under device manager / Ports to
   find out on your system.

![arduino_tools_confirm_correct_com_port_in_drivers](https://user-images.githubusercontent.com/30057649/125969023-8b1fffb6-cc97-41bd-90d4-9c2b83dbf515.jpg)

Configure Copter
================

1. Click on the **APM_Config.h** file tab.
2. Set your frame type (e.g. ``#define FRAME_CONFIG HEXA_FRAME``) in
   order to get the right image for your frame
3. Enable or disable the features you wish in this file.

   Ie if you want to compile with auto tune disabled, simply un-comment
   the line

   :

       //# AUTOTUNE DISABLED // disable the auto tune functionality to save 7k of flash

   To disable Auto Tune which is enabled by default you would change it
   to:

   :

       # AUTOTUNE DISABLED // disable the auto tune functionality to save 7k of flash

   The commented out options are the NON-default and all that needs to
   be done is to un-comment them to use them instead.

4. Save this file and select the file Copter.

   At this point you are ready to compile.

   I would choose Verify for the first attempt.

![arduino_tools_verify_button](https://user-images.githubusercontent.com/30057649/125971820-0ec9ee8c-c042-41d6-b45d-4bfe4f53f3b1.jpg)

Upload to your ArduPilot
========================

1. Then if all is well upload to the autopilot, as shown:

![arduino_tools_upload_ardupilot_button](https://user-images.githubusercontent.com/30057649/125969035-e34f248f-775a-4564-a348-a8a0feff0cb9.png)

   This may take a while…

2. You should end up with the message as shown below.

![arduino_tools_upload_complete_message](https://user-images.githubusercontent.com/30057649/125971738-dd0eaa19-2e43-4766-8fad-13092c8e4140.jpg)
   
3. Configure Your ArduPilot using planner, as normal.

   .. warning::

      The code you have just compiled is now UN-TESTED in your
      configuration. Please use only for testing. If you are not confident,
      please just use mission planner to upload pre-compiled
      code.

Updating your code
==================

Please ensure that the version of code on your PC is the latest version,
use git to update your code to the latest code.

