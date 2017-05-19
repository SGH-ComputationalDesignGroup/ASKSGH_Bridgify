
var sWebSystem = (function () {

    function sWebConverter() {

        this.CrossVectorBysXYZ = function (from, ToA, ToB) {
            var tA = new THREE.Vector3();
            var tB = new THREE.Vector3();
            var cV = new THREE.Vector3();

            tA.sub(this.ToThreeVector(ToA) - this.ToThreeVector(from));
            tB.sub(this.ToThreeVector(ToB) - this.ToThreeVector(from));
            cV.crossVectors(tA, tB);
            cV.normalize();

            return cV;
        }

        this.ToThreeVector = function (svec) {
            return new THREE.Vector3(svec.X, svec.Z, -1 * svec.Y);
        }

        this.TosXYZ = function (tvec) {
            //??
        }

        this.ToTHREEcolor = function (scol) {
            if (scol !== undefined) {
                var tcol = new THREE.Color((scol.R / 255), (scol.G / 255), (scol.B / 255));
                return tcol;
            }
            else {
                return null;
            }
        }

        this.TosColor = function (tcol) {
            //??
        }

        this.ToThreeMesh_sMesh = function (sMesh, edgeColor ,vColor, opacity, castShadow, receiveShadow, renderOrder, reflection, refraction) {
            var tGeo = this.ToThreeBufferGeometry_sMesh(sMesh, vColor);
            //var tGeo = this.toThreeGeometry(sMesh, vColor);

            var tPhyMat = this.ToThreePhysicalMaterial(opacity, reflection, refraction);
            var tMesh = new THREE.Mesh(tGeo, tPhyMat);

            if (renderOrder != undefined) tMesh.renderOrder = renderOrder;
            if (castShadow == true) tMesh.castShadow = true;
            if (receiveShadow == true) tMesh.receiveShadow = true;

            if (sMesh.meshName != undefined) tMesh.userData.name = sMesh.meshName;

            var tMeshes = []
            if (edgeColor != undefined) {
                tMeshes.push(tMesh);
                var tedge = this.GetThreeMeshEdge(tMesh, edgeColor);
                tMeshes.push(tedge);
            }
            else {
                tMeshes.push(tMesh);
            }
            return tMeshes;
        }

        this.ToThreeMesh_sSystem = function (sSystem, colMode, edgeColor , castShadow, receiveShadow, renderOrder) {
            var tMeshes = [];
            var tMat = this.ToThreeTextureMaterial(colMode);
            for (var i = 0; i < sSystem.frameSets.length; ++i) {
                var fs = sSystem.frameSets[i];
                for (var j = 0; j < fs.frames.length; ++j) {
                    var f = fs.frames[j];
                    var tgeo = this.ToThreeBufferGeometry_sFrame(f);
                    var tMesh = new THREE.Mesh(tgeo, tMat);

                    if (renderOrder != undefined) tMesh.renderOrder = renderOrder;
                    if (castShadow == true) tMesh.castShadow = true;
                    if (receiveShadow == true) tMesh.receiveShadow = true;

                    tMesh.userData.name = f.frameName;

                    tMesh.userData.frameObj = f;

                    //first invisible
                    tMesh.visible = false;

                    if (edgeColor != undefined) {
                        tMeshes.push(tMesh);
                        var tedge = this.GetThreeMeshEdge(tMesh, edgeColor);
                        tedge.visible = false;
                        tMeshes.push(tedge);
                    }
                    else {
                        tMeshes.push(tMesh);
                    }
                }
            }
            return tMeshes;
        }

        this.GetThreeMeshEdge = function (tMesh, edgeColor) {
            var tEdgeCol = edgeColor;
            if (tEdgeCol == undefined) tEdgeCol = new THREE.Color(0.4, 0.4, 0.4);
            var tMeshEdge = new THREE.EdgesHelper(tMesh, tEdgeCol);
            tMeshEdge.userData.name = tMesh.userData.name + "_Edge";
            tMeshEdge.userData.isEdge = true;
            tMesh.userData.linkedID = tMeshEdge.id;
            //tMeshEdge.userData.linkedID = tMesh.uuid;
            return tMeshEdge;
        }

        this.ToThreePhysicalMaterial = function (opa, reflect, refract, texture) {

            var o = 1.0;
            if (opa != undefined) o = opa;
            var rl = 0.35;
            if (reflect != undefined) rl = reflect;
            var rr = 1.0;
            if (refract != undefined) rr = refract;
            

            var tMat = new THREE.MeshPhysicalMaterial({
                vertexColors: THREE.VertexColors,
                metalness: 0,
                roughness: 0.4,
                clearCoat: 1.0,
                clearCoatRoughness: 1.0,
                reflectivity: rl,
                transparent: true,
                opacity: o,
                refractionRatio: rr,
                alphaTest: 0.5
            });

            tMat.side = THREE.DoubleSide;

            return tMat;
        }

        this.ToThreeTextureMaterial = function (colMode) {
            var texture;
            var rainbow = false;

            if(colMode == 0 || colMode == 13){
                rainbow = true;
            }

            if (rainbow) {
                texture = new THREE.TextureLoader().load('sWebSystem/LegendColorBar.png');
            }
            else {
                texture = new THREE.TextureLoader().load('sWebSystem/LegendCyanBar.png');
            }
            texture.mapping = THREE.UVMapping;

            var tMat = new THREE.MeshPhysicalMaterial({
                //vertexColors: THREE.VertexColors,
                metalness: 0,
                roughness: 0.4,
                clearCoat: 1.0,
                clearCoatRoughness: 1.0,
                reflectivity: 0.25,
                transparent: true,
                opacity: 1.0,
                refractionRatio: 1.0,
                alphaTest: 0.5,
                map:texture
            });

            tMat.side = THREE.DoubleSide;

            return tMat;
        }

        this.ToThreeBufferGeometry_sFrame = function (sFrame) {
            var geo = new THREE.BufferGeometry();
            
            var faceCount = 2 * (sFrame.results[0].sectionResults.length) * (sFrame.results.length - 1);

            var vPositions = new Float32Array(faceCount * 3 * 3);
            var vUVs = new Float32Array(faceCount * 3 * 2);
            //var vColors = new Uint8Array(faceCount * 3 * 3);

            var verID = 0;
            var uvID = 0;
            for (var i = 0; i < sFrame.results.length-1; ++i) {
                var re_this = sFrame.results[i];
                var re_next = sFrame.results[i + 1];

                for (var j = 0; j < re_this.sectionResults.length; ++j) {
                    var vre0;
                    var vre1;
                    var vre2;
                    var vre3;
                    if (j < re_this.sectionResults.length - 1) {
                        vre0 = re_this.sectionResults[j];
                        vre1 = re_this.sectionResults[j + 1];
                        vre2 = re_next.sectionResults[j + 1];
                        vre3 = re_next.sectionResults[j];
                    }
                    else {
                        vre0 = re_this.sectionResults[j];
                        vre1 = re_this.sectionResults[0];
                        vre2 = re_next.sectionResults[0];
                        vre3 = re_next.sectionResults[j];
                    }

                    var vl0 = this.ToThreeVector(vre0.location);
                    var vl1 = this.ToThreeVector(vre1.location);
                    var vl2 = this.ToThreeVector(vre2.location);
                    var vl3 = this.ToThreeVector(vre3.location);

                    vPositions[verID + 0] = vl0.x;
                    vPositions[verID + 1] = vl0.y;
                    vPositions[verID + 2] = vl0.z;

                    vPositions[verID + 3] = vl1.x;
                    vPositions[verID + 4] = vl1.y;
                    vPositions[verID + 5] = vl1.z;

                    vPositions[verID + 6] = vl3.x;
                    vPositions[verID + 7] = vl3.y;
                    vPositions[verID + 8] = vl3.z;

                    vPositions[verID + 9] =  vl1.x;
                    vPositions[verID + 10] = vl1.y;
                    vPositions[verID + 11] = vl1.z;
                                              
                    vPositions[verID + 12] = vl2.x;
                    vPositions[verID + 13] = vl2.y;
                    vPositions[verID + 14] = vl2.z;
                                              
                    vPositions[verID + 15] = vl3.x;
                    vPositions[verID + 16] = vl3.y;
                    vPositions[verID + 17] = vl3.z;

                    var u0 = 0.0;
                    var u1 = 0.0;
                    var u2 = 0.0;
                    var u3 = 0.0;
                    //0
                    vUVs[uvID + 0] = u0;
                    vUVs[uvID + 1] =  0.5;
                    //1               
                    vUVs[uvID + 2] = u1;
                    vUVs[uvID + 3] = 0.5;
                    //3               
                    vUVs[uvID + 4] = u3;
                    vUVs[uvID + 5] = 0.5;
                    //1               
                    vUVs[uvID + 6] = u1;
                    vUVs[uvID + 7] = 0.5;
                    //2               
                    vUVs[uvID + 8] = u2;
                    vUVs[uvID + 9] = 0.5;
                    //3               
                    vUVs[uvID + 10] = u3;
                    vUVs[uvID + 11] = 0.5;

                    //vColors[verID + 0] = 150;
                    //vColors[verID + 1] = 150;
                    //vColors[verID + 2] = 150;
                    //
                    //vColors[verID + 3] = 150;
                    //vColors[verID + 4] = 150;
                    //vColors[verID + 5] = 150;
                    //
                    //vColors[verID + 6] = 150;
                    //vColors[verID + 7] = 150;
                    //vColors[verID + 8] = 150;
                    //
                    //vColors[verID + 9] = 150;
                    //vColors[verID + 10] = 150;
                    //vColors[verID + 11] = 150;
                    //
                    //vColors[verID + 12] = 150;
                    //vColors[verID + 13] = 150;
                    //vColors[verID + 14] = 150;
                    //
                    //vColors[verID + 15] = 150;
                    //vColors[verID + 16] = 150;
                    //vColors[verID + 17] = 150;

                    verID += 18;
                    uvID += 12;
                }
            }

            geo.addAttribute('position', new THREE.BufferAttribute(vPositions, 3).setDynamic(true));
            //geo.addAttribute('color', new THREE.BufferAttribute(vColors, 3));
            geo.addAttribute('uv', new THREE.BufferAttribute(vUVs, 2).setDynamic(true));

            geo.computeBoundingSphere();

            geo.computeFaceNormals();
            geo.computeVertexNormals();
            
            return geo;
        }

        this.ToThreeBufferGeometry_sMesh = function (sMesh, vColor) {
            var geo = new THREE.BufferGeometry();

            var faceCount = sMesh.faces.length;
            //vertice indices
            var vIndices = new Uint32Array(faceCount * 3);
            //vertice positions
            var vPositions = new Float32Array(faceCount * 3 * 3);
            //vertice normals
            var vNormals = new Float32Array(faceCount * 3 * 3);
            //vertice colors
            var vColors = new Uint8Array(faceCount * 3 * 3);

            var verID = 0;
            for (var i = 0; i < sMesh.faces.length; ++i) {
                var va = sMesh.vertices[sMesh.faces[i].A];
                var vb = sMesh.vertices[sMesh.faces[i].B];
                var vc = sMesh.vertices[sMesh.faces[i].C];

                var tva = this.ToThreeVector(va.location);
                var tvb = this.ToThreeVector(vb.location);
                var tvc = this.ToThreeVector(vc.location);

                vPositions[verID] =     tva.x;
                vPositions[verID + 1] = tva.y;
                vPositions[verID + 2] = tva.z;
                vPositions[verID + 3] = tvb.x;
                vPositions[verID + 4] = tvb.y;
                vPositions[verID + 5] = tvb.z;
                vPositions[verID + 6] = tvc.x;
                vPositions[verID + 7] = tvc.y;
                vPositions[verID + 8] = tvc.z;

                if (va.normal != undefined && vb.normal != undefined && vc.normal != undefined) {
                    vNormals[verID] = va.normal.X;
                    vNormals[verID + 1] = va.normal.Y;
                    vNormals[verID + 2] = va.normal.Z;
                    vNormals[verID + 3] = vb.normal.X;
                    vNormals[verID + 4] = vb.normal.Y;
                    vNormals[verID + 5] = vb.normal.Z;
                    vNormals[verID + 6] = vc.normal.X;
                    vNormals[verID + 7] = vc.normal.Y;
                    vNormals[verID + 8] = vc.normal.Z;
                }
                else {
                    var ca = this.CrossVectorBysXYZ(va, vb, vc);
                    var cb = this.CrossVectorBysXYZ(vb, va, vc);
                    var cc = this.CrossVectorBysXYZ(vc, va, vb);

                    vNormals[verID] =     ca.x;
                    vNormals[verID + 1] = ca.y;
                    vNormals[verID + 2] = ca.z;
                    vNormals[verID + 3] = cb.x;
                    vNormals[verID + 4] = cb.y;
                    vNormals[verID + 5] = cb.z;
                    vNormals[verID + 6] = cc.x;
                    vNormals[verID + 7] = cc.y;
                    vNormals[verID + 8] = cc.z;
                }

                
                //for buffer color use 0-255 int value
                if (vColor) {
                    vColors[verID] = vColor.R;
                    vColors[verID + 1] = vColor.G;
                    vColors[verID + 2] = vColor.B;
                    vColors[verID + 3] = vColor.R;
                    vColors[verID + 4] = vColor.G;
                    vColors[verID + 5] = vColor.B;
                    vColors[verID + 6] = vColor.R;
                    vColors[verID + 7] = vColor.G;
                    vColors[verID + 8] = vColor.B;
                }
                else {
                    if (va.color != undefined && vb.color != undefined && vc.color != undefined) {
                        vColors[verID] = va.color.R;
                        vColors[verID + 1] = va.color.G;
                        vColors[verID + 2] = va.color.B;
                        vColors[verID + 3] = vb.color.R;
                        vColors[verID + 4] = vb.color.G;
                        vColors[verID + 5] = vb.color.B;
                        vColors[verID + 6] = vc.color.R;
                        vColors[verID + 7] = vc.color.G;
                        vColors[verID + 8] = vc.color.B;
                    }
                    else {
                        vColors[verID] = 200;
                        vColors[verID + 1] = 200;
                        vColors[verID + 2] = 200;
                        vColors[verID + 3] = 200;
                        vColors[verID + 4] = 200;
                        vColors[verID + 5] = 200;
                        vColors[verID + 6] = 200;
                        vColors[verID + 7] = 200;
                        vColors[verID + 8] = 200;
                    }
                }
                verID += 9;
            }

            //geo.setIndex(new THREE.BufferAttribute(vIndices, 1));
            geo.addAttribute('position', new THREE.BufferAttribute(vPositions, 3));
            geo.addAttribute('normal', new THREE.BufferAttribute(vNormals, 3));
            geo.addAttribute('color', new THREE.BufferAttribute(vColors, 3));

            geo.computeBoundingSphere();

            geo.computeFaceNormals();
            geo.computeVertexNormals();


            return geo;
        }

        this.ToThreeGeometry = function (sMesh, vColor) {
            var geo = new THREE.Geometry();

            for (var i = 0; i < sMesh.vertices.length; ++i) {
                var tVertex = this.ToThreeVector(sMesh.vertices[i].location);
                geo.vertices.push(tVertex);
            }

            for (var i = 0; i < sMesh.faces.length; ++i) {
                var sFace = sMesh.faces[i];
                var tFace = new THREE.Face3(sFace.A, sFace.B, sFace.C);

                var ca = null;
                var cb = null;
                var cc = null;

                if (vColor) {
                    ca = vColor;
                    cb = vColor;
                    cc = vColor;
                }
                else {
                    ca = this.ToTHREEcolor(sMesh.vertices[sFace.A].color);
                    cb = this.ToTHREEcolor(sMesh.vertices[sFace.B].color);
                    cc = this.ToTHREEcolor(sMesh.vertices[sFace.C].color);
                }

                if (ca == undefined || cb == undefined || cc == undefined) {
                    ca = new THREE.Color(200 / 255, 200 / 255, 200 / 255);
                    cb = new THREE.Color(200 / 255, 200 / 255, 200 / 255);
                    cc = new THREE.Color(200 / 255, 200 / 255, 200 / 255);
                }

                tFace.vertexColors[0] = ca;
                tFace.vertexColors[1] = cb;
                tFace.vertexColors[2] = cc;

                geo.faces.push(tFace);
            }

            geo.computeFaceNormals();
            geo.computeVertexNormals();
            geo.colorsNeedUpdate = true;
            geo.verticesNeedUpdate = true;

            return geo;
        }

    }

    function sSceneHandler() {

        this.ASKSGHProjectName = "You";
        this.ASKSGHType = null;

        this.lScene = null;
        this.lCamera = null;
        this.lControl = null;
        this.lRenderer = null;
        this.lRaycaster = new THREE.Raycaster();
        this.lContainer = null;
        this.lAmbientLight = null;
        this.lSpotLight = null;
        
        this.currentSystem = null;
        this.currentColorMode = null;
        this.currentTextureType = "Rainbow";
        this.shadowMapFactor = 2;
        this.edgeColor = null;
        this.edgeColor_Mesh = null;

        this.sceneObjs = [];
        this.updateFunctions = [];

        var sConverter = new sWebConverter();
        this.documentHandler = new sWebSystem.sDocumentHandler();

        var deformTime = 0.0;
        var animateDeformation = false;
        this.deformSpeed = 40;
        this.deformFactor = 10.0;

        this.isTouchDevice = false;
        this.mouseUI = new THREE.Vector2(0,0);
        this.objHovered = null;
        this.objClicked = null;

        this.ASKSGHProxy = $.connection.sDataCommunication;

        this.InitialASKSGH = function (addSysEdge, addAppendEdge, renderShadow) {
            this.InitiateASKSGHScene(addSysEdge, addAppendEdge, renderShadow);
            this.documentHandler.InitiateASKSGHdocument(this.ASKSGHType, this.ASKSGHProjectName);

        }
        //Three Scene Initiation
        this.InitiateASKSGHScene = function (addSysEdge, addAppendEdge, renderShadow) {
            this.lScene = new THREE.Scene();
            this.lScene.fog = new THREE.FogExp2(0x000000, 0.00025);

            this.lContainer = document.getElementById('THREEScene');
            
            this.lCamera= new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 0.1, 5000);
            this.lCamera.position.x = 8;
            this.lCamera.position.y = 16;
            this.lCamera.position.z = 24;
            this.lCamera.lookAt(new THREE.Vector3(0, 0, 0));

            this.lScene.add(this.lCamera);

            this.lControl = new THREE.OrbitControls(this.lCamera, this.lContainer);
            this.lControl.enableDamping = true;
            this.lControl.dampingFactor = 0.5;
            this.lControl.rotateSpeed = 0.4;
            this.lControl.panSpeed = 0.4;

            this.lControl.target.set(0, 0, 0);
            this.lControl.update();


            this.lRenderer = new THREE.WebGLRenderer({
                antialias: true
            });
            this.lRenderer.setClearColor(0x333333, 1);
            this.lRenderer.setPixelRatio(window.devicePixelRatio);
            this.lRenderer.setSize(window.innerWidth, window.innerHeight);
            this.lRenderer.gammaInput = true;
            this.lRenderer.gammaOutput = true;

            if (renderShadow) {
                this.lRenderer.shadowMap.type = THREE.PCFSoftShadowMap;
                this.lRenderer.shadowMap.enabled = true;
                this.lRenderer.shadowMapSoft = true;

                this.AddSpotLight("spotlight", new THREE.Vector3(30, 30, 30), this.shadowMapFactor);
            }
            else {
                this.AddSpotLight("spotlight", new THREE.Vector3(30, 30, 30));
            }

            this.lContainer.appendChild(this.lRenderer.domElement);

            this.lAmbientLight = new THREE.AmbientLight(0x101010);
            this.lAmbientLight.intensity = 10.0;
            this.lScene.add(this.lAmbientLight);


            window.addEventListener('resize', this.onWindowResize(), false);

            this.isTouchDevice = this.CheckIsTouchDevice();
            if (this.isTouchDevice) {
                

            }
            else {
                this.lRenderer.domElement.addEventListener('mousemove', this.onMouseMove(), false);
                this.lRenderer.domElement.addEventListener('mousedown', this.onMouseDown(), false);
                this.lRenderer.domElement.addEventListener('mouseup', this.onMouseUp(), false);
            }

            if (addSysEdge) {
                this.edgeColor = new THREE.Color(0.5, 0.5, 0.5);
            }
            if (addAppendEdge) {
                this.edgeColor_Mesh = new THREE.Color(0.5, 0.5, 0.5);
            }
        }

        this.AddSpotLight = function (spotLightName, position, shadowMapSizeFactor) {
            this.lSpotLight = new THREE.SpotLight(0xffffff, 1.5);
            this.lSpotLight.position.copy(position);

            if (shadowMapSizeFactor != undefined) {
                this.lSpotLight.angle = Math.PI / 5;
                this.lSpotLight.penumbra = 0.3;
                this.lSpotLight.position.set(10, 10, 5);
                this.lSpotLight.castShadow = true;
                this.lSpotLight.shadow.camera.near = 8;
                this.lSpotLight.shadow.camera.far = 1000;
                this.lSpotLight.shadow.mapSize.width = 2048 * shadowMapSizeFactor;
                this.lSpotLight.shadow.mapSize.height = 2048 * shadowMapSizeFactor;

            }
            this.lSpotLight.intensity = 0.7;
            this.lSpotLight.userData = spotLightName;

            this.lScene.add(this.lSpotLight);
        }
        
        this.AddBaseGrid = function(gridName, size, step) {
            var grid = new THREE.GridHelper(size, step, 0x333333, 0x111111);
            grid.userData.name = gridName;
            grid.position.y -= 0.001;
            this.lScene.add(grid);
        }

        this.AddBaseAxisLines = function (axisName, size) {
            var axis = new THREE.AxisHelper(size);
            axis.rotateX(Math.PI * -0.5);

            axis.userData.name = axisName;
            this.lScene.add(axis);
        }


        //System Management
        this.UpdatesSystemColor = function (colMode, colThreshold) {

            if (this.currentSystem == undefined) return;

            var tMat;
            var nextTextureType = this.AwareTextureType(colMode);
            if (this.currentTextureType !== nextTextureType) {
                this.currentTextureType = nextTextureType;
                tMat = sConverter.ToThreeTextureMaterial(colMode);
            }
            var range = this.GetsSystemDataRange(colMode);
            var that = this;

            this.lScene.children.forEach(function (to) {
                if (to instanceof (THREE.Mesh) && to.userData.frameObj != undefined) {
                    if (tMat != undefined) {
                        to.material = tMat;
                        to.material.needsUpdate = true;
                    }

                    var tg = to.geometry;
                    var sFrame = to.userData.frameObj;
                    
                    var uvID = 0;
                    for (var i = 0; i < sFrame.results.length - 1; ++i) {
                        var re_this = sFrame.results[i];
                        var re_next = sFrame.results[i + 1];
                        for (var j = 0; j < re_this.sectionResults.length; ++j) {
                            var vre0;
                            var vre1;
                            var vre2;
                            var vre3;
                            if (j < re_this.sectionResults.length - 1) {
                                vre0 = that.GetsFrameDataAt(sFrame, colMode, i, j);
                                vre1 = that.GetsFrameDataAt(sFrame, colMode, i, j+1); 
                                vre2 = that.GetsFrameDataAt(sFrame, colMode, i+1, j+1); 
                                vre3 = that.GetsFrameDataAt(sFrame, colMode, i+1, j); 
                            }
                            else {
                                vre0 = that.GetsFrameDataAt(sFrame, colMode, i, j); //vre0 = re_this.sectionResults[j];
                                vre1 = that.GetsFrameDataAt(sFrame, colMode, i, 0);// vre1 = re_this.sectionResults[0];
                                vre2 = that.GetsFrameDataAt(sFrame, colMode, i + 1, 0);// vre2 = re_next.sectionResults[0];
                                vre3 = that.GetsFrameDataAt(sFrame, colMode, i + 1, j); //vre3 = re_next.sectionResults[j];
                            }

                            var u0 = that.GetNormalizedResult(vre0, range, colMode, colThreshold);
                            var u1 = that.GetNormalizedResult(vre1, range, colMode, colThreshold);
                            var u2 = that.GetNormalizedResult(vre2, range, colMode, colThreshold);
                            var u3 = that.GetNormalizedResult(vre3, range, colMode, colThreshold);
                            //0
                            tg.attributes.uv.array[uvID + 0] = u0;
                            tg.attributes.uv.array[uvID + 1] = 0.5;
                            //1               
                            tg.attributes.uv.array[uvID + 2] = u1;
                            tg.attributes.uv.array[uvID + 3] = 0.5;
                            //3               
                            tg.attributes.uv.array[uvID + 4] = u3;
                            tg.attributes.uv.array[uvID + 5] = 0.5;
                            //1               
                            tg.attributes.uv.array[uvID + 6] = u1;
                            tg.attributes.uv.array[uvID + 7] = 0.5;
                            //2               
                            tg.attributes.uv.array[uvID + 8] = u2;
                            tg.attributes.uv.array[uvID + 9] = 0.5;
                            //3               
                            tg.attributes.uv.array[uvID + 10] = u3;
                            tg.attributes.uv.array[uvID + 11] = 0.5;

                            uvID += 12;
                        }
                    }
                    tg.attributes.uv.needsUpdate = true;
                }
            });
        }
        
        this.AnimateDeformSystem = function (animate, factor, speed) {
            if (factor != undefined) this.deformFactor = factor;
            if (speed != undefined) this.deformSpeed = speed;

            if (animate) {
                animateDeformation = true;
            }
            else {
                animateDeformation = false;
                deformTime = 0.0;
                this.DeformSystem(0.0);
            }
        }

        this.DeformSystemAnimation = function () {
            var gradientVal = this.deformFactor * (Math.abs(Math.cos(deformTime * this.deformSpeed) - 1));

            this.DeformSystem(gradientVal);
            deformTime += 0.001;
        }

        this.DeformSystem = function (deformFactor) {
            if (this.currentSystem == undefined) return;

            var that = this;

            this.lScene.children.forEach(function (to) {
                if (to instanceof (THREE.Mesh) && to.userData.frameObj != undefined) {

                    var tg = to.geometry;
                    tg.dynamic = true;

                    var sFrame = to.userData.frameObj;

                    var verID = 0;
                    for (var i = 0; i < sFrame.results.length - 1; ++i) {
                        var re_this = sFrame.results[i];
                        var re_next = sFrame.results[i + 1];
                        for (var j = 0; j < re_this.sectionResults.length; ++j) {
                            var vre0;
                            var vre1;
                            var vre2;
                            var vre3;

                            if (j < re_this.sectionResults.length - 1) {
                                vre0 = that.GetsFrameVertexDisplacementAt(sFrame, i, j, deformFactor);//re_this.sectionResults[j];
                                vre1 = that.GetsFrameVertexDisplacementAt(sFrame, i, j + 1, deformFactor);//re_this.sectionResults[j + 1];
                                vre2 = that.GetsFrameVertexDisplacementAt(sFrame, i + 1, j + 1, deformFactor);//re_next.sectionResults[j + 1];
                                vre3 = that.GetsFrameVertexDisplacementAt(sFrame, i + 1, j, deformFactor);//re_next.sectionResults[j];
                            }
                            else {
                                vre0 = that.GetsFrameVertexDisplacementAt(sFrame, i, j, deformFactor);//re_this.sectionResults[j];
                                vre1 = that.GetsFrameVertexDisplacementAt(sFrame, i, 0, deformFactor);//re_this.sectionResults[0];
                                vre2 = that.GetsFrameVertexDisplacementAt(sFrame, i + 1, 0, deformFactor);//re_next.sectionResults[0];
                                vre3 = that.GetsFrameVertexDisplacementAt(sFrame, i + 1, j, deformFactor);//re_next.sectionResults[j];
                            }

                            tg.attributes.position.array[verID + 0] =  vre0.x;
                            tg.attributes.position.array[verID + 1] =  vre0.y;
                            tg.attributes.position.array[verID + 2] =  vre0.z;
                            
                            tg.attributes.position.array[verID + 3] =  vre1.x;
                            tg.attributes.position.array[verID + 4] =  vre1.y;
                            tg.attributes.position.array[verID + 5] =  vre1.z;
                            
                            tg.attributes.position.array[verID + 6] =  vre3.x;
                            tg.attributes.position.array[verID + 7] =  vre3.y;
                            tg.attributes.position.array[verID + 8] =  vre3.z;
                            
                            tg.attributes.position.array[verID + 9] =  vre1.x;
                            tg.attributes.position.array[verID + 10] = vre1.y;
                            tg.attributes.position.array[verID + 11] = vre1.z;
                            
                            tg.attributes.position.array[verID + 12] = vre2.x;
                            tg.attributes.position.array[verID + 13] = vre2.y;
                            tg.attributes.position.array[verID + 14] = vre2.z;
                            
                            tg.attributes.position.array[verID + 15] = vre3.x;
                            tg.attributes.position.array[verID + 16] = vre3.y;
                            tg.attributes.position.array[verID + 17] = vre3.z;

                            verID += 18;
                        }
                    }

                    tg.attributes.position.needsUpdate = true;
                }
            });
        }

        this.ImportJsonSystem = function (jsonSystem , castShadow, receiveShadow, renderOrder) {
            var defaultColorMode = jsonSystem.systemSettings.currentCheckType;
            var defaultColorThreshold = jsonSystem.systemSettings.currentStressThreshold_pascal;
            if (defaultColorMode == sWebSystem.eColorMode.Deflection) {
                defaultColorThreshold = jsonSystem.systemSettings.currentDeflectionThreshold_mm;
            }

            var tMeshes = sConverter.ToThreeMesh_sSystem(jsonSystem, defaultColorMode, this.edgeColor, castShadow, receiveShadow, renderOrder);
            var that = this;
            tMeshes.forEach(function (tm) {
                if (tm.userData.isEdge == undefined) {
                    that.sceneObjs.push(tm);
                }
                that.lScene.add(tm);
            });
            this.currentSystem = jsonSystem;
            this.UpdatesSystemColor(defaultColorMode, defaultColorThreshold);

            this.ZoomToFit(this.sceneObjs);

            if (jsonSystem.meshes != undefined) {
                for (var i = 0; i < jsonSystem.meshes.length; ++i) {
                    this.ImportJsonsMesh(jsonSystem.meshes[i], this.edgeColor_Mesh, 1.0, null, castShadow, receiveShadow, renderOrder);
                }
            }
        }

        this.ImportLocalJsonsSystem = function (systemName, edgeColor, castShadow, receiveShadow, renderOrder) {
            //if (edgeColor != undefined) {
            //    this.edgeColor = new THREE.Color(0.5, 0.5, 0.5);
            //}
            var that = this;

            stHan.ToggleControl(false);
            stHan.documentHandler.ToggleLoadingElement(true);
            $.getJSON('../Jsons/' + systemName + '.json', function (sys) {
                that.ImportJsonSystem(sys, castShadow, receiveShadow, renderOrder);
                that.RenderUpdatedSystem();
            });
        }

        this.ImportJsonsMesh = function (jsonMesh, edgeColor, opacity, customColor, castShadow, receiveShadow, renderOrder, reflaction, refraction) {
            var that = this;
            var op = jsonMesh.opacity;
            if (opacity != undefined) op = opacity;

            var tMeshes = sConverter.ToThreeMesh_sMesh(jsonMesh, edgeColor, customColor, opacity, castShadow, receiveShadow, renderOrder, reflaction, refraction);
            tMeshes.forEach(function (tm) {
                that.lScene.add(tm);
                if (tm.userData.isEdge == undefined) {
                    that.sceneObjs.push(tm);
                }
            });
        }

        this.ImportLocalJsonsMesh = function (jsonName, edgeColor, opacity, customColor, castShadow, receiveShadow, renderOrder, reflaction, refraction) {
            var that = this;
            //if (edgeColor != undefined) this.edgeColor_Mesh = edgeColor;
            $.getJSON('../Jsons/' + jsonName + '.json', function (sm) {
                var op = sm.opacity;
                if (opacity != undefined) op = opacity;
                that.ImportJsonsMesh(sm, that.edgeColor_Mesh, op, customColor, castShadow, receiveShadow, renderOrder, reflaction, refraction);
            });
        }

        this.RenderUpdateSystemPromise = function () {
            var deferred = new $.Deferred();
            var that = this;
            this.RenderUpdatedSystem(function (e) {
                deferred.resolve(e);
            });

            return deferred.promise();
        }

        this.RenderUpdatedSystem = function () {
            var that = this;
           setTimeout(function () {
               that.ShowObjects(that.sceneObjs);
               setTimeout(function () {
                   that.FadeOutLoadingElement();
               }, 500);
           }, 1000);
        }

        this.FadeOutLoadingElement = function () {
           this.documentHandler.ToggleLoadingElement(false);
           this.ToggleControl(true);
        }

        this.GetsSystemDataRange = function (colMode) {

            var minVal = 0;
            var maxVal = 0;

            var sysRe = this.currentSystem.systemResults;

            switch (colMode) {
                case sWebSystem.eColorMode.NONE:
                    
                    break;
                case sWebSystem.eColorMode.Stress_Combined_Absolute:
                    minVal = 0.0;
                    maxVal = sysRe.stressCombinedAbs;
                    break;
                case sWebSystem.eColorMode.Deflection:
                    var dv = sConverter.ToThreeVector(sysRe.deflectionMax_Abs_mm);
                    minVal = 0.0;
                    maxVal = dv.length();
                    break;
                case sWebSystem.eColorMode.Force_X:
                    minVal = sysRe.forceMax_Negative.X;
                    maxVal = sysRe.forceMax_Positive.X;
                    break;
                case sWebSystem.eColorMode.Force_Y:
                    minVal = sysRe.forceMax_Negative.Y;
                    maxVal = sysRe.forceMax_Positive.Y;
                    break;
                case sWebSystem.eColorMode.Force_Z:
                    minVal = sysRe.forceMax_Negative.Z;
                    maxVal = sysRe.forceMax_Positive.Z;
                    break;
                case sWebSystem.eColorMode.Moment_X:
                    minVal = sysRe.momentMax_Negative.X;
                    maxVal = sysRe.momentMax_Positive.X;
                    break;
                case sWebSystem.eColorMode.Moment_Y:
                    minVal = sysRe.momentMax_Negative.X;
                    maxVal = sysRe.momentMax_Positive.X;
                    break;
                case sWebSystem.eColorMode.Moment_Z:
                    minVal = sysRe.momentMax_Negative.Z;
                    maxVal = sysRe.momentMax_Positive.Z;
                    break;
                case sWebSystem.eColorMode.Stress_Axial_X:
                    minVal = sysRe.momentMax_Negative.X;
                    maxVal = sysRe.momentMax_Positive.X;
                    break;
                case sWebSystem.eColorMode.Stress_Axial_Y:
                    var t = null;
                    break;
                case sWebSystem.eColorMode.Stress_Axial_Z:
                    var t = null;
                    break;
                case sWebSystem.eColorMode.Stress_Moment_X:
                    var t = null;
                    break;
                case sWebSystem.eColorMode.Stress_Moment_Y:
                    var t = null;
                    break;
                case sWebSystem.eColorMode.Stress_Moment_Z:
                    var t = null;
                    break;
            }

            return new THREE.Vector2(minVal, maxVal);
        }

        this.GetsFrameVertexDisplacementAt = function (sFrame, i, j, du) {
            var oriv = sConverter.ToThreeVector(sFrame.results[i].sectionResults[j].location);
            var defv = new THREE.Vector3();
            if (du == undefined) du = 1.0;
            defv.x = oriv.x + sFrame.results[i].sectionResults[j].deflection_mm.X * 0.001 * du;
            defv.y = oriv.y + sFrame.results[i].sectionResults[j].deflection_mm.Z * 0.001 * du;
            defv.z = oriv.z + sFrame.results[i].sectionResults[j].deflection_mm.Y * -0.001 * du;

            return defv;
        }

        this.GetsFrameDataAt = function (sFrame, colMode, i, j) {
            var dataAt;
            switch (colMode) {
                case sWebSystem.eColorMode.Deflection:
                    var defv = sConverter.ToThreeVector(sFrame.results[i].sectionResults[j].deflection_mm);
                    dataAt = defv.length();
                    break;
                case sWebSystem.eColorMode.Stress_Combined_Absolute:
                    dataAt = sFrame.results[i].sectionResults[j].stress_Combined;
                    break;
                case sWebSystem.eColorMode.Stress_Axial_X:
                    dataAt = sFrame.results[i].sectionResults[j].stress_Axial_X;
                    break;
                case sWebSystem.eColorMode.Stress_Axial_Y:
                    dataAt = sFrame.results[i].sectionResults[j].stress_Axial_Y;
                    break;
                case sWebSystem.eColorMode.Stress_Axial_Z:
                    dataAt = sFrame.results[i].sectionResults[j].stress_Axial_Z;
                    break;
                case sWebSystem.eColorMode.Stress_Moment_X:
                    dataAt = sFrame.results[i].sectionResults[j].stress_Moment_X;
                    break;
                case sWebSystem.eColorMode.Stress_Moment_Y:
                    dataAt = sFrame.results[i].sectionResults[j].stress_Moment_Y;
                    break;
                case sWebSystem.eColorMode.Stress_Moment_Z:
                    dataAt = sFrame.results[i].sectionResults[j].stress_Moment_Z;
                    break;
                case sWebSystem.eColorMode.Force_X:
                    dataAt = sFrame.results[i].force.X;
                    break;
                case sWebSystem.eColorMode.Force_Y:
                    dataAt = sFrame.results[i].force.Y;
                    break;
                case sWebSystem.eColorMode.Force_Z:
                    dataAt = sFrame.results[i].force.Z;
                    break;
                case sWebSystem.eColorMode.Moment_X:
                    dataAt = sFrame.results[i].moment.X;
                    break;
                case sWebSystem.eColorMode.Moment_Y:
                    dataAt = sFrame.results[i].moment.Y;
                    break;
                case sWebSystem.eColorMode.Moment_Z:
                    dataAt = sFrame.results[i].moment.Z;
                    break;

            }
            return dataAt;
        }

        this.GetNormalizedResult = function (value, oriRange, colMode, colThreshold) {
            //check normalize works properly

            oriRange.y = colThreshold;
            var norVal;
            if (colMode < 7 || colMode == 13) {
                if (value >= colThreshold) {
                    norVal = 1.0;
                }
                else {
                    norVal = 0.1 + ((value - oriRange.x) * (0.9) / (oriRange.y - oriRange.x));
                }
            }
            else {
                var valueAbs = Math.abs(value);
                var maxRange = Math.max(Math.abs(oriRange.x), Math.abs(oriRange.y));

                if (value > 0.0) {
                    if (valueAbs >= colThreshold) {
                        norVal = 1.0;
                    }
                    else {
                        norVal = 0.55 + ((valueAbs) * (0.45) / (maxRange));
                    }
                }
                else {
                    if (valueAbs >= colThreshold) {
                        norVal = 0.1;
                    }
                    else {
                        norVal = 0.1 + 0.55 - (0.1 + ((valueAbs) * (0.45) / (maxRange)));
                    }
                }
            }
        
            return norVal;
        }

        this.AwareTextureType = function (colMode) {
            if (colMode == 0 || colMode == 13) {
                return "Rainbow";
            }
            else {
                return "CyanRed";
            }
        }

        //Scene Management

        //highlight object ideas

        this.RemoveObjectsPromise = function (obj, meshOnly) {
            var deferred = new $.Deferred();
            var that = this;
            this.RemoveObjects(obj, meshOnly, function (e) {
                deferred.resolve(e);
            });
            return deferred.promise();
        }

        this.RemoveObjects = function (obj, meshOnly) {
            if (this.sceneObjs == undefined && this.sceneObjs.length == 0) return

            var that = this;
            if (obj instanceof Array) {
                obj.forEach(function (o) {
                    if (o instanceof THREE.Mesh) {
                        if (meshOnly == undefined || meshOnly == false) {
                            var linkedEdge = that.lScene.getObjectById(o.userData.linkedID);
                            if (linkedEdge != undefined) {
                                linkedEdge.material.dispose();
                                linkedEdge.geometry.dispose();
                                that.lScene.remove(linkedEdge);
                            }
                        }
                        o.material.dispose();
                        o.geometry.dispose();
                        that.lScene.remove(o);
                    }
                });
            }
            else {
                if (obj instanceof THREE.Mesh) {
                    if (meshOnly == undefined || meshOnly == false) {
                        var linkedEdge = that.lScene.getObjectById(obj.userData.linkedID);
                        if (linkedEdge != undefined) {
                            linkedEdge.material.dispose();
                            linkedEdge.geometry.dispose();
                            that.lScene.remove(linkedEdge);
                        }
                    }
                    obj.material.dispose();
                    obj.geometry.dispose();
                    that.lScene.remove(obj);
                }
            }
        }

        this.ShowObjects = function (obj, meshOnly) {
            var that = this;

            if (obj instanceof Array) {
                obj.forEach(function (o) {
                    if (o instanceof THREE.Mesh) {
                        o.visible = true;
                        if (meshOnly == undefined || meshOnly == false) {
                            var linkedEdge = that.lScene.getObjectById(o.userData.linkedID);
                            if (linkedEdge) {
                                linkedEdge.visible = true;
                            }
                        }
                    }
                });
            }
            else {
                if (obj instanceof THREE.Mesh) {
                    obj.visible = true;
                    if (meshOnly == undefined || meshOnly == false) {
                        var linkedEdge = that.lScene.getObjectById(obj.userData.linkedID);
                        if (linkedEdge) {
                            linkedEdge.visible = true;
                        }
                    }
                }
            }
        }

        this.HideObjects = function (obj) {
            if (obj instanceof Array) {
                obj.forEach(function (o) {
                    if (o instanceof THREE.Mesh) {
                        o.visible = false;
                    }
                });
            }
            else {
                if (obj instanceof THREE.Mesh) {
                    obj.visible = false;
                }
            }
        }

      //  this.RemoveSceneObjects = function (obj) {
      //      for (var i in this.sceneObjs) {
      //          if (this.sceneObjs[i] == obj) {
      //              this.sceneObjs.splice(i, 1);
      //              break;
      //          }
      //      }
      //  }

        this.GetCameraShadowHelper = function () {
            var helper = new THREE.CameraHelper(this.lSpotLight.shadow.camera);
            this.lScene.add(helper);
        }

        this.GetObjectsByUserName = function (userName, identicalOnly) {
            var objs = []
            this.lScene.traverse(function (obj) {
                if (obj.userData.name != undefined) {
                    if (identicalOnly) {
                        if (obj.userData.name === userName) {
                            objs.push(obj);
                        }
                    }
                    else {
                        if (obj.userData.name.includes(userName)) {
                            objs.push(obj);
                        }
                    }
                }
            });
            return objs;
        }

        this.GetObjectSize = function (obj) {
            var maxSize = 0.0;
            var objCen = new THREE.Vector3(0, 0, 0);
            var count = 0;

            if (obj instanceof Array) {
                obj.forEach(function (m) {
                    if (m instanceof THREE.Mesh) {
                        var g = m.geometry;
                        g.computeBoundingSphere();
                        var cen = g.boundingSphere.center;

                        objCen.x += cen.x;
                        objCen.y += cen.y;
                        objCen.z += cen.z;
                        count++;
                    }
                });
                objCen.divideScalar(count);
                obj.forEach(function (m) {
                    if (m instanceof THREE.Mesh) {
                        var g = m.geometry;
                        g.computeBoundingSphere();
                        var cen = g.boundingSphere.center;

                        var dis = objCen.distanceTo(cen);
                        if (dis > maxSize) maxSize = dis;
                    }
                });
            }
            else {
                if (obj instanceof THREE.Mesh) {
                    var g = obj.geometry;
                    objCen = g.boundingSphere.center;
                    maxSize = g.boundingSphere.radius;
                }
            }
            var sizeInfo = {
                size : maxSize,
                cenVec : objCen
            }
            return sizeInfo;
        }

        this.ZoomToFit = function (objs) {

            var sizeInfo = this.GetObjectSize(objs);

            var fov = this.lCamera.fov * (Math.PI / 180);
            var distance = Math.abs(sizeInfo.size / Math.sin(fov / 2));
            //var distance = sizeInfo.size;

            var fac = 0.5;
            var newCamLoc = new THREE.Vector3(
                sizeInfo.cenVec.x + fac * distance,
                sizeInfo.cenVec.y + fac * distance,
                sizeInfo.cenVec.z + fac * distance
                );

            this.UpdateCameraLocation(newCamLoc, sizeInfo.cenVec);
            if(this.lSpotLight != undefined){
                this.UpdateLightLocation(this.lSpotLight ,newCamLoc, sizeInfo.cenVec);
            }
        }

        this.UpdateCameraLocation = function (cameraPosition, cameraLookAt) {
            this.lCamera.position.copy(cameraPosition);
            this.lCamera.lookAt(cameraLookAt);
            this.lControl.target.copy(cameraLookAt);
            this.lControl.update();
        }

        this.UpdateLightLocation = function (lLight,lightPosition,lightLookAt , near, far) {
            lLight.position.copy(lightPosition);
            lLight.lookAt(lightLookAt);
           //
           // if (near != undefined) {
           //     lLight.castShadow = true;
           //     lLight.shadow.camera.near = near;
           // }
           // if (far != undefined) {
           //     lLight.castShadow = true;
           //     lLight.shadow.camera.far = far;
           // }

        }
        
        this.GetRayIntersection = function (obj) {
            this.lRaycaster.setFromCamera(this.mouseUI, this.lCamera);
            var intersects;
            if (obj instanceof Array) {
                intersects = this.lRaycaster.intersectObjects(obj, true);
            } else {
                intersects = this.lRaycaster.intersectObject(obj, true);
            }
            return intersects;
        }

        //Scene Animation
        this.AninmateScene = function () {
            requestAnimationFrame(this.AninmateScene.bind(this));
            this.UpdateScene();
            this.lRenderer.render(this.lScene, this.lCamera);
        }

        this.UpdateScene = function () {
            if (animateDeformation) {
               this.DeformSystemAnimation();
            }

            this.updateFunctions.forEach(function (func) {
                func.call();
            });
        }

        //Event Handlers
        this.ToggleControl = function (toggle) {
            if (toggle) {
                this.lControl.enabled = true;
            }
            else {
                this.lControl.enabled = false;
            }
        }

        this.CheckIsTouchDevice = function () {
            return 'ontouchstart' in window // works on most browsers 
        || 'onmsgesturechange' in window; // works on ie10
        }

        this.onWindowResize = function () {
            var that = this;
            return function () {
                windowHalfX = window.innerWidth / 2;
                windowHalfY = window.innerHeight / 2;

                that.lCamera.aspect = window.innerWidth / window.innerHeight;
                that.lCamera.updateProjectionMatrix();
                that.lRenderer.setSize(window.innerWidth, window.innerHeight);
            }
        }

        this.onMouseMove = function () {
            var that = this;
            return function (event) {
                event.preventDefault();

                that.mouseUI.x = (event.clientX / window.innerWidth) * 2 - 1;
                that.mouseUI.y = -(event.clientY / window.innerHeight) * 2 + 1

                var int_d = that.GetRayIntersection(that.sceneObjs);
                if (int_d.length > 0) {
                    that.objHovered = int_d[0].object;
                    $('#ASKSGH').css('cursor', 'pointer');
                }
                else {
                    $('#ASKSGH').css('cursor', 'default');
                    objHovered = null;
                }
            }
        }

        this.onMouseDown = function () {
            var that = this;
            return function (event) {
                event.preventDefault();

                if (event.button == 0) {
                    var int_d = that.GetRayIntersection(that.sceneObjs);
                    if (int_d.length > 0) {
                        that.objClicked = int_d[0].object;

                    }
                    else {
                        that.objClicked = null;
                    }
                }
            }
        }

        this.onMouseUp = function () {
            var that = this;
            return function (event) {
                event.preventDefault();

                if (event.button == 0) {
                    that.objClicked = null;
                }
            }
        }

        //signalR
        this.ProxyServerOn = function () {
            
            $.connection.hub.logging = true;
            $.connection.hub.start().done(function () {
                console.debug('Connected to ASKSGH Server');
                //clientConnectionID = $.connection.hub.id;
                //console.debug(clientConnectionID);
            })

            this.RecievesSystemFromServer();
        }

        //Try to implement Promise...correctly
        this.RecievesSystemFromServer = function () {
            var that = this;
            this.ASKSGHProxy.client.receiveSystemFromServer = function (jsonSystem) {
                // if (clientGlobal) {

                that.ToggleControl(false);
                that.documentHandler.ToggleLoadingElement(true);
                setTimeout(function () {
                    that.currentSystem = JSON.parse(jsonSystem);
                    if (that.sceneObjs != undefined && that.sceneObjs.length > 0) {
                        that.RemoveObjectsPromise(that.sceneObjs).then(
                            that.ImportJsonSystem(that.currentSystem)
                            )
                        that.RenderUpdateSystemPromise().then(
                            that.documentHandler.UpdateRelaventInformation(that.currentSystem, that.ASKSGHType)
                        );
                    }
                    else {
                        that.ImportJsonSystem(that.currentSystem);
                        that.RenderUpdateSystemPromise().then(
                            that.documentHandler.UpdateRelaventInformation(that.currentSystem, that.ASKSGHType)
                        );
                    }
                }, 1000);
            };
        }

    }


    function sDocumentHandler() {

        this.InitiateASKSGHdocument = function (titleType, subTitle) {
            this.AddLoadingElement(titleType);
            this.AddASKSGHrightSideUI(titleType, subTitle);

            if (titleType == sWebSystem.eASKSGHType.ASKSGH_Colorify_Dyanmic) {
                this.AddSystemInfoElement_Colorify(titleType);
            }
            else if (titleType == sWebSystem.eASKSGHType.ASKSGH_Colorify_Static) {
                this.AddSystemInfoElement_Colorify(titleType);
            }
            else if (titleType == sWebSystem.eASKSGHType.ASKSGH_Gridify) {
                this.AddSystemInfoElement_Gridify();
            }
            else if (titleType == sWebSystem.eASKSGHType.ASKSGH_Webify) {

            }

            this.AddLowerUI(titleType);
            $('#lowerUI').removeClass('customHidden');
        }

        this.UpdateRelaventInformation = function (jsonSystem, titleType) {
            if (titleType == sWebSystem.eASKSGHType.ASKSGH_Colorify_Dyanmic) {
                this.UpdateSystemInfo_Colorify(jsonSystem);
            }
            else if (titleType == sWebSystem.eASKSGHType.ASKSGH_Colorify_Static) {
                this.UpdateSystemInfo_Colorify(jsonSystem);
            }
            else if (titleType == sWebSystem.eASKSGHType.ASKSGH_Gridify) {
                this.UpdateSystemInfo_Gridify(jsonSystem);
            }
            else if (titleType == sWebSystem.eASKSGHType.ASKSGH_Webify) {

            }
            $('#systemInfo').removeClass('customHidden');
        }

        //colorify
        this.UpdateSystemInfo_Colorify = function (jsonSystem) {

            var currentUnit = jsonSystem.systemSettings.systemOriUnit;
            var estDeflection = jsonSystem.estimatedMaxD * 1000; //m to mm
            var estTotalWeight = jsonSystem.estimatedWeight;
            var currentCase = jsonSystem.systemSettings.currentCase;

            $('#info1').html("Load Case/Combination : " + currentCase);
            
            if (currentUnit == "Meters") {
                $('#info2').html("Estimated Maximum Deflection : " + Math.round(100 * estDeflection) / 100 + "mm");
                $('#info3').html("Estimated Total Weight : " + Math.round(100 * estTotalWeight) / 100 + "N");
                //if (info4 != undefined) $('#est_weight_isolated').html("Estimated Total Weight : " + Math.round(100 * estIsoWeight) / 100 + "N").removeClass('customHidden');
            }
            else {
                $('#info2').html("Estimated Maximum Deflection : " + Math.round(100 * estDeflection * 0.0393701) / 100 + "in");
                $('#info3').html("Estimated Total Weight : " + Math.round(100 * estTotalWeight * 0.000224809) / 100 + "kips(f)");
                //if (info4 != undefined) $('#est_weight_isolated').html(estIsoWeight);
            }

            $('#statusIcon').removeClass('grey').removeClass('help').removeClass('circle').removeClass('outline').removeClass('red').removeClass('frown').removeClass('orange').removeClass('meh').addClass('smile').addClass('green');
            $('#statusInfo').html("System is Stable!");
        }

        this.AddSystemInfoElement_Colorify = function (titleType) {
            var html = "";
            var iconClass = 'ui big inverted grey help circle outline icon';
            if (titleType == sWebSystem.eASKSGHType.ASKSGH_Colorify_Dyanmic) {
                html = "Please Upload Your Design";
            }
            else {
                html = "Please Wait Until We Load Your Structure";
            }

            var leftInfo = $(document.createElement('div')).attr('id', 'leftInfo').css({
                'float': 'left', 'position': 'fixed', 'top': '4%', 'left': '30px'
            });
            var h2ele = $(document.createElement('h2')).addClass('ui grey inverted left aligned header').appendTo(leftInfo);
            var cont = $(document.createElement('div')).addClass('content').append(
                $(document.createElement('i')).addClass(iconClass).attr('id', 'statusIcon')
                ).append(
                $(document.createElement('span')).attr('id', 'statusInfo').css({
                    'font-size': '15px', 'margin-top': '2px', 'margin-bottom': '0px'
                }).html(html)
                ).appendTo(h2ele);

            var psdiv = $(document.createElement('div')).attr('id','systemInfo').append(
            $(document.createElement('p')).attr('id', 'info1').css({ 'font-size': '12px', 'margin-top': '15px', 'margin-left': '-5px', 'text-indent': '1em' })
            ).append(
            $(document.createElement('p')).attr('id', 'info2').css({ 'font-size': '12px', 'margin-top': '5px', 'margin-left': '-5px', 'text-indent': '1em','margin-bottom': '0px' })
            ).append(
            $(document.createElement('p')).attr('id', 'info3').css({ 'font-size': '12px', 'margin-top': '5px', 'margin-left': '-5px', 'text-indent': '1em' })
            ).append(
            $(document.createElement('p')).attr('id', 'info4').css({ 'font-size': '12px', 'margin-top': '5px', 'margin-left': '-5px', 'text-indent': '1em' })
            ).addClass('customHidden').appendTo(h2ele);
             
            $(document.body).append(leftInfo);
        }
        //
        //gridify
        this.UpdateSystemInfo_Gridify = function (jsonSystem) {
            //this is tricky... 
            //sBuildingSystem VS sSystem?


        }

        this.AddSystemInfoElement_Gridify = function () {
            var html = "Please Upload Your Building";
            var iconClass = 'ui big inverted grey help circle outline icon';

            var leftInfo = $(document.createElement('div')).attr('id', 'leftInfo').css({
                'float': 'left', 'position': 'fixed', 'top': '4%', 'left': '30px'
            });
            var h2ele = $(document.createElement('h2')).addClass('ui grey inverted left aligned header').appendTo(leftInfo);
            var cont = $(document.createElement('div')).addClass('content').append(
                $(document.createElement('i')).addClass(iconClass).attr('id', 'statusIcon')
                ).append(
                $(document.createElement('span')).attr('id', 'statusInfo').css({
                    'font-size': '15px', 'margin-top': '2px', 'margin-bottom': '0px'
                }).html(html)
                ).appendTo(h2ele);

            //info1 : checked type? LRFD or ASD?
            //info2 : estimated total weight
            //info3 : number of beams
            //info4 : number of girder
            //info5 : number of column

            var psdiv = $(document.createElement('div')).attr('id', 'systemInfo').append(
            $(document.createElement('p')).attr('id', 'info1').css({ 'font-size': '12px', 'margin-top': '15px', 'margin-left': '-5px', 'text-indent': '1em' })
            ).append(
            $(document.createElement('p')).attr('id', 'info2').css({ 'font-size': '12px', 'margin-top': '5px', 'margin-left': '-5px', 'text-indent': '1em', 'margin-bottom': '0px' })
            ).append(
            $(document.createElement('p')).attr('id', 'info3').css({ 'font-size': '12px', 'margin-top': '5px', 'margin-left': '-5px', 'text-indent': '1em' })
            ).append(
            $(document.createElement('p')).attr('id', 'info4').css({ 'font-size': '12px', 'margin-top': '5px', 'margin-left': '-5px', 'text-indent': '1em' })
            ).append(
            $(document.createElement('p')).attr('id', 'info5').css({ 'font-size': '12px', 'margin-top': '5px', 'margin-left': '-5px', 'text-indent': '1em' })
            ).appendTo(h2ele);

            $(document.body).append(leftInfo);
        }
        //
        //webify

        //
        //all fy
        this.AddLoadingElement = function (titleType) {
            var loadingHTML = this.GetLoadingHTML(titleType);
            $(document.createElement('div')).addClass('ui dimmer').attr('id', 'loading').append(
                $(document.createElement('div')).attr('id', 'loadingText').css('font-size', '20px').addClass('ui text loader').html(loadingHTML)
                ).css('background-color', 'rgba(0, 0, 0, 0.75)').appendTo($('#THREEScene'));
        }

        this.ToggleLoadingElement = function (toggle) {
            $('#loading').dimmer({
                opacity: 0.9,
                closable : false,
                duration: {
                    show: 1500,
                    hide: 1500
                }
            });

            if (toggle) {
                    $('#loading').dimmer('show').addClass('dimmerOn');
            }
            else {
                if ($('#loading').hasClass('dimmerOn')) {
                    $('#loading').dimmer('hide').removeClass('dimmerOn');
                }
            }
        }

        this.AddLowerUI = function (titleType) {
            var lowerui = this.GetLowerUIHTML(titleType)

            var lui = $(document.createElement('div')).addClass('grid customHidden').attr('id', 'lowerUI').css(
                {
                    'left': '2%',
                    'width': '96%',
                    'height': '8%',
                    'position': 'fixed',
                    'bottom': '3%'
                }
            );
            var grid = $(document.createElement('div')).addClass('ui equal width grid');
            grid.append(
                $(document.createElement('div')).addClass('column').append(
                $(document.createElement('i')).addClass('huge icons').attr('id', 'LowerIcon1')
                    .append($(document.createElement('i')).addClass(lowerui.class_1_main))
                    .append($(document.createElement('i')).addClass(lowerui.class_1_sub))
                )
            )
            grid.append(
                $(document.createElement('div')).addClass('column').append(
                $(document.createElement('i')).addClass('huge icons').attr('id', 'LowerIcon2')
                    .append($(document.createElement('i')).addClass(lowerui.class_2_main))
                    .append($(document.createElement('i')).addClass(lowerui.class_2_sub))
                )
            )
            grid.append(
                $(document.createElement('div')).addClass('column').append(
                $(document.createElement('i')).addClass('huge icons').attr('id', 'LowerIcon3')
                    .append($(document.createElement('i')).addClass(lowerui.class_3_main))
                    .append($(document.createElement('i')).addClass(lowerui.class_3_sub))
                )
            )
            grid.append(
                $(document.createElement('div')).addClass('column').append(
                $(document.createElement('i')).addClass('huge icons').attr('id', 'LowerIcon4')
                    .append($(document.createElement('i')).addClass(lowerui.class_4_main))
                    .append($(document.createElement('i')).addClass(lowerui.class_4_sub).attr('id', 'playIcon'))
                )
            )

            lui.append(grid);
            $(document.body).append(lui);
        }

        this.GetLowerUIHTML = function (titleType) {
            var lowerui = {
                class_1_main: null,
                class_1_sub:  null,
                class_2_main: null,
                class_2_sub:  null,
                class_3_main: null,
                class_3_sub: null,
                class_4_main: null,
                class_4_sub: null
            };

            if (titleType == sWebSystem.eASKSGHType.ASKSGH_Colorify_Dyanmic) {
                lowerui.class_1_main = 'ui inverted teal cube icon';
                lowerui.class_1_sub =  'corner inverted orange checkmark icon';
                lowerui.class_2_main = 'ui inverted red cube icon';
                lowerui.class_2_sub =  'corner inverted olive sort content ascending icon';
                lowerui.class_3_main = 'ui inverted olive cube icon';
                lowerui.class_3_sub =  'corner inverted green help icon';
                lowerui.class_4_main = 'ui inverted blue cube icon';
                lowerui.class_4_sub =  'corner inverted red paint play icon';
            }
            else if (titleType == sWebSystem.eASKSGHType.ASKSGH_Colorify_Static) {
                lowerui.class_1_main = 'ui inverted teal cube icon';
                lowerui.class_1_sub = 'corner inverted orange add icon';
                lowerui.class_2_main = 'ui inverted red cube icon';
                lowerui.class_2_sub = 'corner inverted olive minus icon';
                lowerui.class_3_main = 'ui inverted olive cube icon';
                lowerui.class_3_sub =  'corner inverted green photo icon';
                lowerui.class_4_main = 'ui inverted blue cube icon';
                lowerui.class_4_sub =  'corner inverted red paint brush icon';
            }
            else if (titleType == sWebSystem.eASKSGHType.ASKSGH_Gridify) {
                lowerui.class_1_main = "";
                lowerui.class_1_sub = "";
                lowerui.class_2_main = "";
                lowerui.class_2_sub = "";
                lowerui.class_3_main = "";
                lowerui.class_3_sub = "";
                lowerui.class_4_main = "";
                lowerui.class_4_sub = "";
            }
            else if (titleType == sWebSystem.eASKSGHType.ASKSGH_Webify) {

            }
            return lowerui;
        }

        this.AddASKSGHrightSideUI = function (titleType, subTitle) {
            var rightInfo = $(document.createElement('div')).attr('id', 'rightInfo').css({
                'float': 'right',
                'position': 'fixed',
                'top': '4%',
                'right': '30px',
                'width': '500px'
            })
            
            var h2c = $(document.createElement('h2')).addClass('ui grey inverted right aligned header');

            var titleContainer = this.GetTitleHTML(titleType, subTitle);
            var cont = $(document.createElement('div')).addClass('content').html(titleContainer.mainTitle);;
            
            $(document.createElement('p')).css({
                "font-size": "12px",
                "margin-top": "2px",
                "margin-bottom": "0px"
            }).appendTo(cont).html("Powered by SGH Computational Design Group");
            $(document.createElement('p')).css({
                "font-size": "12px"
            }).appendTo(cont).html(titleContainer.subTitle);
            h2c.append(cont);

            $(document.createElement('div')).addClass('ui image').attr('id', 'rightLogo').css({
                "top":"2px",
                "width": "50px",
                "margin-left": "10px"
            }).appendTo(h2c).html(
                '<a href="http://www.sgh.com" class="ui image"><img src="sWebSystem/sgh_logo.png" id="rightLogoImage"></a>'
            );

            rightInfo.append(h2c);
            $(document.body).append(rightInfo);
        }

        this.GetTitleHTML = function (titleType, subTitle) {
            var titleContainer = { mainTitle : "", subTitle : "" };

            if (titleType == 0 || titleType == 1) {
                titleContainer.mainTitle = 'ASKSGH.<span style="color:#ff9b9b">C</span><span style="color:#ccb9a8">O</span><span style="color:#addfc2">L</span><span style="color:#addfc2">O</span><span style="color:#a4e3e2">R</span><span style="color:#a4c3e6">I</span><span style="color:#81a3dc">F</span><span style="color:#456dbe">Y(</span><span style="color:darkgray">' + subTitle + '</span><span style="color:#456dbe">)</span>';
                if (titleType == 0) {
                    titleContainer.subTitle = "Bridgified on ASKSGH/COLORIFY/" + subTitle;
                }
                else {
                    titleContainer.subTitle = "Colorified on ASKSGH/COLORIFY/" + subTitle;
                }
            }
            else if (titleType == 2) {
                titleContainer.mainTitle = 'ASKSGH.<span style="color:#66b9ff">W</span><span style="color:#79c1ff">E</span><span style="color:#90ccff">B</span><span style="color:#a8d7ff">I</span><span style="color:#c0e2ff">F</span><span style="color:#c0e2ff">Y</span><span style="color:#5779c1">(</span> <span style="color:darkgray">' + subTitle + '</span> <span style="color:#5779c1">)</span>';
                titleContainer.subTitle = "Webified on ASKSGH/WEBIFY/" + subTitle;
                }
            else if(titleType == 3){
                titleContainer.mainTitle = 'ASKSGH.<span style="color:#ff695e">G</span><span style="color:#ff766c">R</span><span style="color:#ff857c">I</span><span style="color:#ff948c">D</span><span style="color:#ffa6a0">I</span><span style="color:#ffb7b2">F</span><span style="color:#ffc5c1">Y</span><span style="color:#ec8b5c">(</span> <span style="color:darkgray">' + subTitle + '</span> <span style="color:#ec8b5c">)</span>';
                titleContainer.subTitle = "Gridified on ASKSGH/GRIDIFY/" + subTitle;
            }
            return titleContainer;
        }

        this.GetLoadingHTML = function (titleType) {
            if (titleType == 0 || titleType == 1) {
                return '<span>WE ARE </span> <span style="color:#ff9b9b">C</span><span style="color:#ccb9a8">O</span><span style="color:#addfc2">L</span><span style="color:#addfc2">O</span><span style="color:#a4e3e2">R</span><span style="color:#a4c3e6">I</span><span style="color:#81a3dc">F</span><span style="color:#456dbe">Y</span><span>ing YOUR STRUCTURE. <br />PLEASE WAIT :D</span>';
            }
            else if (titleType == 2) {
                return '<span>WE ARE </span><span style="color:#66b9ff">W</span><span style="color:#79c1ff">E</span><span style="color:#90ccff">B</span><span style="color:#a8d7ff">I</span><span style="color:#c0e2ff">F</span><span style="color:#c0e2ff">Y</span><span>ing YOUR DESIGN. <br />PLEASE WAIT :D</span>';
            }
            else if (titleType == 3) {
                return '<span>WE ARE </span><span style="color:#ff695e">G</span><span style="color:#ff766c">R</span><span style="color:#ff857c">I</span><span style="color:#ff948c">D</span><span style="color:#ffa6a0">I</span><span style="color:#ffb7b2">F</span><span style="color:#ffc5c1">Y</span><span>ing YOUR BUILDING. <br />PLEASE WAIT :D</span>';
            }
        }
    }

    var eASKSGHType = {
        "ASKSGH_Colorify_Dyanmic": 0,
        "ASKSGH_Colorify_Static": 1,
        "ASKSGH_Webify": 2,
        "ASKSGH_Gridify": 3,
        "ASKSGH_Graphify":4
    };

    var eColorMode = {
        "Stress_Combined_Absolute": 0,
        "Stress_Moment_X": 1,
        "Stress_Moment_Y": 2,
        "Stress_Moment_Z": 3,
        "Stress_Axial_X": 4,
        "Stress_Axial_Y": 5,
        "Stress_Axial_Z": 6,
        "Moment_X": 7,
        "Moment_Y": 8,
        "Moment_Z": 9,
        "Force_X": 10,
        "Force_Y": 11,
        "Force_Z": 12,
        "Deflection": 13,
        "NONE": 14
    };

    return {
        sWebConverter: sWebConverter,
        sSceneHandler: sSceneHandler,
        sDocumentHandler:sDocumentHandler,
        eColorMode: eColorMode,
        eASKSGHType:eASKSGHType
    }

}());

