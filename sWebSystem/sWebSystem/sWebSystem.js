
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

        this.ToThreeMesh_sMesh = function (sMesh, visible, edgeColor ,vColor, opacity, castShadow, receiveShadow, renderOrder, reflection, refraction) {
            var tGeo = this.ToThreeBufferGeometry_sMesh(sMesh, vColor);
            //var tGeo = this.toThreeGeometry(sMesh, vColor);

            var tPhyMat = this.ToThreePhysicalMaterial(opacity, reflection, refraction);
            var tMesh = new THREE.Mesh(tGeo, tPhyMat);

            if (renderOrder != undefined) tMesh.renderOrder = renderOrder;
            if (visible == false) tMesh.visible = false;
            if (castShadow) tMesh.castShadow = true;
            if (receiveShadow) tMesh.receiveShadow = true;

            if (sMesh.meshName != undefined) tMesh.userData.name = sMesh.meshName;

            var tMeshes = []
            if (edgeColor != undefined) {
                tMeshes.push(tMesh);
                tMeshes.push(this.GetThreeMeshEdge(tMesh, edgeColor));
            }
            else {
                tMeshes.push(tMesh);
            }
            return tMeshes;
        }

        this.ToThreeMesh_sSystem = function (sSystem, colMode, colThreshold, edgeColor , castShadow, receiveShadow, renderOrder) {
            var tMeshes = [];
            var tMat = this.ToThreeTextureMaterial(colMode);
            for (var i = 0; i < sSystem.frameSets.length; ++i) {
                var fs = sSystem.frameSets[i];
                for (var j = 0; j < fs.frames.length; ++j) {
                    var f = fs.frames[j];
                    var tgeo = this.ToThreeBufferGeometry_sFrame(f);
                    var tMesh = new THREE.Mesh(tgeo, tMat);

                    if (renderOrder != undefined) tMesh.renderOrder = renderOrder;
                    if (castShadow) tMesh.castShadow = true;
                    if (receiveShadow) tMesh.receiveShadow = true;

                    tMesh.userData.name = f.frameName + '_' + f.frameID;

                    tMesh.userData.frameObj = f;

                    if (edgeColor != undefined) {
                        tMeshes.push(tMesh);
                        tMeshes.push(this.GetThreeMeshEdge(tMesh, edgeColor));
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
            tMeshEdge.userData.linked = tMesh.userData.name;
            return tMeshEdge;
        }

        this.ToThreePhysicalMaterial = function (opa, reflect, refract) {

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

            var tMat = new THREE.MeshBasicMaterial({ map: texture });

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
        //
        this.lScene = null;
        this.lCamera = null;
        this.lControl = null;
        this.lRenderer = null;
        this.lContainer = null;
        this.lAmbientLight = null;
        this.lSpotLight = null;
        //
        this.currentSystem = null;
        this.currentColorMode = null;
        this.currentTextureType = "Rainbow";

        this.meshObjs = [];
        this.updateFunctions = [];
        var sConverter = new sWebConverter();
        


        this.InitiateASKSGHScene = function (renderShadow) {
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
                this.lRenderer.shadowMapType = THREE.PCFSoftShadowMap;
                this.lRenderer.shadowMap.enabled = true;
                this.lRenderer.shadowMapSoft = true;
            }
            this.lContainer.appendChild(this.lRenderer.domElement);

            this.lAmbientLight = new THREE.AmbientLight(0x303030);
            this.lScene.add(this.lAmbientLight);

            window.addEventListener('resize', this.onWindowResize(), false);
        }

        this.AddSpotLight = function (spotLightName, position, shadowMapSizeFactor) {
            this.lSpotLight = new THREE.SpotLight(0xffffff, 1.5);
            this.lSpotLight.position.copy(position);

            if (shadowMapSizeFactor != undefined) {
                this.lSpotLight.castShadow = true;
                this.lSpotLight.shadowDarkness = 0.3;
                this.lSpotLight.shadow.camera.near = 1;
                this.lSpotLight.shadow.camera.far = 2;

                //gSpotLight.shadow.bias = 0.0001;

                this.lSpotLight.shadow.mapSize.width = 2048 * shadowMapSizeFactor;
                this.lSpotLight.shadow.mapSize.height = 2048 * shadowMapSizeFactor;
            }
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

        this.UpdatesSystemColor = function (colMode, colThreshold) {

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
        
        this.ImportLocalJsonsSystem = function (systemName, defaulColorMode, defaultColorThreshold, edgeColor, castShadow, receiveShadow, renderOrder) {
            var that = this;
            $.getJSON('../Jsons/' + systemName + '.json', function (sys) {
                var tMeshes = sConverter.ToThreeMesh_sSystem(sys, defaulColorMode, defaultColorThreshold, edgeColor, castShadow, receiveShadow, renderOrder);
                tMeshes.forEach(function (tm) {
                    that.lScene.add(tm);
                });
                that.currentSystem = sys;
            });
        }

        this.ImportLocalJsonsMesh = function (jsonName, visible, edgeColor, opacity, customColor, castShadow, receiveShadow, renderOrder, reflaction, refraction) {
            var that = this;
            $.getJSON('../Jsons/' + jsonName + '.json', function (sm) {
                var tMeshes = sConverter.ToThreeMesh_sMesh(sm, visible, edgeColor, customColor, opacity, castShadow, receiveShadow, renderOrder, reflaction, refraction);
                tMeshes.forEach(function (tm) {
                    that.lScene.add(tm);
                });
            });
        }

        this.GetsSystemDataRange = function (colMode) {

            var minVal;
            var maxVal;

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
                    var t = null;
                    break;
                case sWebSystem.eColorMode.Force_Y:
                    var t = null;
                    break;
                case sWebSystem.eColorMode.Force_Z:
                    var t = null;
                    break;
                case sWebSystem.eColorMode.Moment_X:
                    var t = null;
                    break;
                case sWebSystem.eColorMode.Moment_Y:
                    var t = null;
                    break;
                case sWebSystem.eColorMode.Moment_Z:
                    var t = null;
                    break;
                case sWebSystem.eColorMode.Stress_Axial_X:
                    var t = null;
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
                        norVal = 0.55 - ((valueAbs) * (0.45) / (maxRange));
                    }
                }
            }
        
            return norVal;
        }

        this.RemoveObjects = function (objs) {
            //review this code...
            for (var i = 0; i < objs.length; ++i) {
                objs[i].material.dispose();
                objs[i].geometry.dispose();
                this.lScene.remove(objs[i]);
            }
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

        this.UpdateCameraLocation = function (cameraPosition, cameraLookAt) {
            this.lCamera.position.copy(cameraPosition);
            this.lCamera.lookAt(cameraLookAt);
            this.lControl.target.copy(cameraLookAt);
            this.lControl.update();
        }

        this.AwareTextureType = function (colMode) {
            if (colMode == 0 || colMode == 13) {
                return "Rainbow";
            }
            else {
                return "CyanRed";
            }
        }
        //animation
        this.AninmateScene = function () {
            requestAnimationFrame(this.AninmateScene.bind(this));
            this.UpdateScene();
            this.lRenderer.render(this.lScene, this.lCamera);
        }

        this.UpdateScene = function () {
            this.updateFunctions.forEach(function (func) {
                func.call();
            });
        }

        //event handlers
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

    }

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
        eColorMode : eColorMode
    }

}());

