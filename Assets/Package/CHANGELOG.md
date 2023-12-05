# Change Log

## [0.5.6-beta] - Dec 05, 2023
- Fix for the state selector window displaying empty groups when searching. 
- Fix for groups found in searches being folded.
- Fix for property container width not spanning full node width when there are no properties

## [0.5.5-beta] - Nov 29, 2023
-Exposed the current state in the StateMachine and in the StateMachineController

## [0.5.4-beta] - Nov 29, 2023
- fix issue with progress bar not being displayed
- refactored the StateMachineGraphView to better separate concerns

## [0.5.3-beta] - Nov 27, 2023
- Seperated the graph orientation from port orientation to better separate editor scripts from runtime.

## [0.5.2-beta] - Nov 27, 2023
- Fix for introduced build errors

## [0.5.1-beta] - Nov 27, 2023
- Hid the samples directory (which still needs actual samples)
- 
## [0.5.0-beta] - Nov 26, 2023
- Support for vertical and flipped directional relay nodes
- The state selection menu now appears where the mouse was when it was opened
- Fix for input and output ports being able to connect to their own kind
- Fix for the create state button becoming broken when docking/undocking the state machine editor
- Added a fix for edge positions when dragging out
- Fix for inverted edges. This was a real challenge to understand.
- Fix for bug where connection data could be lost when switching state machines
- Fixed the adding and positioning of new edges by moving the creation process into the StateEdgeListener. 
  This required the graph views position to be used to offset the new edge.
- Support for custom state nodes
- Added revised node colours
- Added support for inherited attributes

## [0.4.3-beta] - Nov 24, 2023
- Hotfix for hidden state icons
- 
## [0.4.2-beta] - Nov 24, 2023
- The StateSelectorWindow now focuses on the ToolbarSearchField for instant searching
- Added a more dedicated service for icons and moved some icon directories around
- Added an editor tool to toggle the samples directory tilde prefix to make my life easier

## [0.4.1-beta] - Nov 23, 2023
- Filtered out abstract states from the state selector 

## [0.4.0-beta] - Nov 23, 2023
- Added new state icons. Built in states are displayed at the top.
- Added focus loss to close StateSelectorWindow. 
- Added new state selector styling. 
- Added state selector state grouping.

## [0.3.6-alpha] - Nov 23, 2023
- Graph view is now drawn on editor update even in edit mode
    - This has the advantage of removing the progress state when exiting runtime

## [0.3.5-alpha] - Nov 23, 2023
- Critical hotfix for the graphview not updating

## [0.3.4-alpha] - Nov 23, 2023
- Fixed persistence of graph view position and scale

## [0.3.3-alpha] - Nov 22, 2023
- Fix for nodes not being drawn when the graph view is first opened

## [0.3.2-alpha] - Nov 22, 2023
- Hotfix for broken edge persistence
- 
## [0.3.1-alpha] - Nov 22, 2023
- Removed some test code for manipulating edge rendering

## [0.3.0-alpha] - Nov 21, 2023
- Improved relay nodes
- Fix for scale and position of graph view for new statemachines
- Fix for blank port labels
- Removed entry port from entry node
- Enforced use of an entry node
- Removed option to set entry node

## [0.2.1-alpha] - Nov 21, 2023
- Fix for SubStateMachine not cleaning up when exiting

## [0.2.0-alpha] - Nov 21, 2023
- Added support for inherited properties in States
- Added a delay and end node
- Fix a bug in the StateMachineController when sub state machines complete

## [0.1.4-alpha] - Nov 21, 2023
- Changed the directory structure and names to avoid Editor namespace conflic

## [0.1.3-alpha] - Nov 21, 2023
- Updated the package.json unity version
- Added support for creating nodes when dropping a ghost edge

## [0.1.2-alpha] - Nov 20, 2023
-  Added support for switching the displayed statemachine when entering and exiting a sub state machine
-  Converted node titles from pascal to title case
-  Added support for custom node colours and label text

## [0.1.1-alpha] - Nov 20, 2023
-  Added changelog, readme and package.json

## [0.1.0-alpha] - Nov 20, 2023
-  Fix for attempting to load graph view state from a null statemachine

